import { useEffect, useState } from 'react';
import { studentService } from '../services/student.service';
import type { CourseDto, StudentProfileDto } from '../types';
import { toast } from 'react-hot-toast';
import { BookOpen, Check, Loader2, AlertCircle, Info } from 'lucide-react';

export const CourseCatalog = () => {
  const [courses, setCourses] = useState<CourseDto[]>([]);
  const [profile, setProfile] = useState<StudentProfileDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [isEnrolling, setIsEnrolling] = useState(false);
  const [selectedCourses, setSelectedCourses] = useState<number[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [courseRes, profileRes] = await Promise.all([
          studentService.getAvailableCourses(),
          studentService.getProfile()
        ]);
        if (courseRes.success) setCourses(courseRes.data.items);
        if (profileRes.success) setProfile(profileRes.data);
      } catch (error) {
        toast.error('Failed to load catalog');
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  const toggleCourse = (id: number) => {
    setSelectedCourses(prev => 
      prev.includes(id) ? prev.filter(c => c !== id) : [...prev, id]
    );
  };

  const handleEnroll = async () => {
    if (selectedCourses.length === 0) return;
    if (!profile?.isApproved) {
      toast.error('You cannot enroll until your profile is approved.');
      return;
    }

    setIsEnrolling(true);
    try {
      const result = await studentService.enroll(selectedCourses);
      if (result.success) {
        toast.success('Successfully enrolled in selected courses!');
        setSelectedCourses([]);
      }
    } catch (error) {
      toast.error('Enrollment failed');
    } finally {
      setIsEnrolling(false);
    }
  };

  if (loading) return <div className="animate-pulse space-y-4 shadow-sm p-8 bg-white rounded-3xl h-96"></div>;

  const isApproved = profile?.isApproved;

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 dark:text-white">Course Catalog</h1>
          <p className="text-slate-500 dark:text-slate-400">Discover and enroll in upcoming academic courses</p>
        </div>
        
        <button
          onClick={handleEnroll}
          disabled={selectedCourses.length === 0 || isEnrolling || !isApproved}
          className="bg-primary-600 hover:bg-primary-700 disabled:bg-slate-200 disabled:text-slate-400 text-white font-bold py-3 px-8 rounded-2xl shadow-lg shadow-primary-500/20 transition-all flex items-center justify-center gap-2 active:scale-95"
        >
          {isEnrolling ? <Loader2 className="animate-spin" size={20} /> : <Check size={20} />}
          Enroll Selected ({selectedCourses.length})
        </button>
      </div>

      {!isApproved && (
        <div className="bg-amber-50 border border-amber-100 rounded-2xl p-4 flex items-center gap-3 text-amber-800">
          <AlertCircle size={24} className="shrink-0" />
          <p className="text-sm font-medium">
            Enrollment is restricted. Your profile is currently <span className="underline font-bold">{profile?.approvalStatus}</span>. You can browse courses but enrollment requires administrator approval.
          </p>
        </div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {courses.map(course => {
          const isSelected = selectedCourses.includes(course.id);
          return (
            <div 
              key={course.id} 
              onClick={() => isApproved && toggleCourse(course.id)}
              className={`p-6 bg-white dark:bg-slate-900 border-2 rounded-3xl transition-all cursor-pointer relative group ${
                isSelected ? 'border-primary-500 shadow-md shadow-primary-500/10' : 'border-slate-50 dark:border-slate-800 hover:border-slate-200'
              } ${!isApproved ? 'opacity-70 grayscale-[0.5] cursor-not-allowed' : ''}`}
            >
              <div className="flex justify-between items-start mb-4">
                <div className={`p-3 rounded-2xl transition-colors ${
                  isSelected ? 'bg-primary-600 text-white' : 'bg-slate-100 dark:bg-slate-800 text-slate-500'
                }`}>
                  <BookOpen size={24} />
                </div>
                {isSelected && (
                  <div className="bg-primary-600 text-white p-1 rounded-full">
                    <Check size={14} />
                  </div>
                )}
              </div>
              
              <h3 className="text-lg font-bold text-slate-900 dark:text-white mb-1">{course.name}</h3>
              <div className="flex items-center gap-2 mb-3">
                <span className="text-[10px] font-bold uppercase tracking-widest bg-slate-100 dark:bg-slate-800 px-2 py-0.5 rounded text-slate-600">{course.code}</span>
                <span className="text-[10px] font-bold uppercase tracking-widest bg-primary-50 text-primary-600 px-2 py-0.5 rounded">{course.credits} Credits</span>
              </div>
              
              <p className="text-sm text-slate-500 dark:text-slate-400 line-clamp-2 mb-4">
                {course.description || 'Join this course to expand your academic knowledge and skills.'}
              </p>
              
              <div className="flex items-center gap-3 pt-4 border-t border-slate-50 dark:border-slate-800">
                <div className="w-8 h-8 rounded-full bg-slate-200 flex items-center justify-center text-xs font-bold text-slate-600">
                  {course.teacherName?.[0] || 'T'}
                </div>
                <div className="flex-1 overflow-hidden">
                  <p className="text-xs font-bold text-slate-900 dark:text-white truncate">{course.teacherName || 'Faculty Member'}</p>
                  <p className="text-[10px] text-slate-500">Instructor</p>
                </div>
              </div>

              {!isApproved && (
                <div className="absolute inset-0 bg-transparent flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
                  <div className="bg-slate-900/80 text-white px-4 py-2 rounded-xl flex items-center gap-2 text-xs backdrop-blur-sm">
                    <Info size={14} /> Approval Required
                  </div>
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
};
