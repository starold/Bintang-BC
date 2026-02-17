import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { studentService } from '../services/student.service';
import type { StudentProfileDto } from '../types';
import { toast } from 'react-hot-toast';
import { User, Mail, MapPin, Phone, Calendar, Save, Loader2 } from 'lucide-react';

export const StudentProfile = () => {
  const [profile, setProfile] = useState<StudentProfileDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  
  const { register, handleSubmit, reset } = useForm();

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const result = await studentService.getProfile();
        if (result.success) {
          setProfile(result.data);
          reset({
            fullName: result.data.fullName,
            address: result.data.address,
            phoneNumber: result.data.phoneNumber,
            dateOfBirth: result.data.dateOfBirth.split('T')[0]
          });
        }
      } catch (error) {
        toast.error('Failed to load profile');
      } finally {
        setLoading(false);
      }
    };
    fetchProfile();
  }, [reset]);

  const onSubmit = async (data: any) => {
    setIsSaving(true);
    try {
      const result = await studentService.updateProfile(data);
      if (result.success) {
        toast.success('Profile updated successfully');
        setProfile(result.data);
      } else {
        toast.error(result.message || 'Update failed');
      }
    } catch (error) {
      toast.error('Failed to update profile');
    } finally {
      setIsSaving(false);
    }
  };

  if (loading) return <div className="animate-pulse space-y-4 shadow-sm p-8 bg-white rounded-3xl h-96"></div>;

  return (
    <div className="max-w-4xl mx-auto space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <div>
        <h1 className="text-2xl font-bold text-slate-900 dark:text-white">My Profile</h1>
        <p className="text-slate-500 dark:text-slate-400">View and update your personal information</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="lg:col-span-1 space-y-6">
          <div className="bg-white dark:bg-slate-900 p-8 rounded-3xl border border-slate-100 dark:border-slate-800 shadow-sm text-center">
            <div className="w-24 h-24 rounded-3xl bg-primary-100 dark:bg-primary-900/30 text-primary-600 dark:text-primary-400 flex items-center justify-center text-4xl font-bold mx-auto mb-4">
              {profile?.fullName[0]}
            </div>
            <h2 className="text-xl font-bold text-slate-900 dark:text-white uppercase truncate">{profile?.fullName}</h2>
            <p className="text-sm text-slate-500 mb-6">{profile?.email}</p>
            <div className={`inline-flex px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider ${
              profile?.isApproved ? 'bg-emerald-50 text-emerald-600' : 'bg-amber-50 text-amber-600'
            }`}>
              {profile?.approvalStatus}
            </div>
          </div>
        </div>

        <div className="lg:col-span-2">
          <form onSubmit={handleSubmit(onSubmit)} className="bg-white dark:bg-slate-900 p-8 rounded-3xl border border-slate-100 dark:border-slate-800 shadow-sm space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-1.5">
                <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider ml-1">Full Name</label>
                <div className="relative group">
                  <User className="absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400 group-focus-within:text-primary-500" size={16} />
                  <input {...register('fullName', { required: true })} className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border-none rounded-2xl text-sm focus:ring-2 focus:ring-primary-500/20 outline-none" />
                </div>
              </div>
              <div className="space-y-1.5">
                <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider ml-1">Birth Date</label>
                <div className="relative group">
                  <Calendar className="absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400 group-focus-within:text-primary-500" size={16} />
                  <input type="date" {...register('dateOfBirth', { required: true })} className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border-none rounded-2xl text-sm focus:ring-2 focus:ring-primary-500/20 outline-none" />
                </div>
              </div>
              <div className="space-y-1.5">
                <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider ml-1">Phone Number</label>
                <div className="relative group">
                  <Phone className="absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400 group-focus-within:text-primary-500" size={16} />
                  <input {...register('phoneNumber')} className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border-none rounded-2xl text-sm focus:ring-2 focus:ring-primary-500/20 outline-none" />
                </div>
              </div>
              <div className="space-y-1.5">
                <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider ml-1">Email (Read-only)</label>
                <div className="relative">
                  <Mail className="absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-300" size={16} />
                  <input value={profile?.email} readOnly className="w-full pl-10 pr-4 py-3 bg-slate-100 dark:bg-slate-800/50 border-none rounded-2xl text-sm text-slate-400 outline-none cursor-not-allowed" />
                </div>
              </div>
            </div>

            <div className="space-y-1.5">
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider ml-1">Address</label>
              <div className="relative group">
                <MapPin className="absolute left-3.5 top-3 text-slate-400 group-focus-within:text-primary-500" size={16} />
                <textarea {...register('address', { required: true })} rows={3} className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border-none rounded-2xl text-sm focus:ring-2 focus:ring-primary-500/20 outline-none resize-none" />
              </div>
            </div>

            <button
              disabled={isSaving}
              type="submit"
              className="w-full md:w-auto bg-primary-600 hover:bg-primary-700 text-white font-bold py-3 px-8 rounded-2xl shadow-lg shadow-primary-500/20 transition-all flex items-center justify-center gap-2 active:scale-95 disabled:opacity-70"
            >
              {isSaving ? <Loader2 className="animate-spin" size={20} /> : <Save size={20} />}
              Update Profile
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};
