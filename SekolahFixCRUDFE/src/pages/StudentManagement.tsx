import { useEffect, useState } from 'react';
import { adminService } from '../services/admin.service';
import type { StudentProfileDto } from '../types';
import { toast } from 'react-hot-toast';
import { Check, X, Search, Filter, Mail, MapPin, Calendar } from 'lucide-react';

export const StudentManagement = () => {
  const [students, setStudents] = useState<StudentProfileDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');

  const fetchStudents = async () => {
    setLoading(true);
    try {
      const result = await adminService.getStudents();
      if (result.success) {
        setStudents(result.data.items);
      }
    } catch (error) {
      toast.error('Failed to fetch students');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStudents();
  }, []);

  const handleApprove = async (id: number) => {
    try {
      const result = await adminService.approveStudent(id);
      if (result.success) {
        toast.success('Student approved successfully');
        fetchStudents();
      }
    } catch (error) {
      toast.error('Approval failed');
    }
  };

  const handleReject = async (id: number) => {
    if (!confirm('Are you sure you want to reject this student?')) return;
    try {
      const result = await adminService.rejectStudent(id);
      if (result.success) {
        toast.success('Student rejected');
        fetchStudents();
      }
    } catch (error) {
      toast.error('Rejection failed');
    }
  };

  const filteredStudents = students.filter(s => 
    s.fullName.toLowerCase().includes(searchTerm.toLowerCase()) || 
    s.email.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 dark:text-white">Student Management</h1>
          <p className="text-slate-500 dark:text-slate-400">Review and manage student registrations</p>
        </div>

        <div className="flex items-center gap-3">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
            <input
              type="text"
              placeholder="Search students..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 pr-4 py-2 bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-xl text-sm focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 outline-none w-full md:w-64 transition-all"
            />
          </div>
          <button className="p-2 border border-slate-200 dark:border-slate-800 rounded-xl bg-white dark:bg-slate-900 text-slate-600 dark:text-slate-400 hover:bg-slate-50">
            <Filter size={18} />
          </button>
        </div>
      </div>

      <div className="bg-white dark:bg-slate-900 rounded-2xl border border-slate-100 dark:border-slate-800 shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left">
            <thead>
              <tr className="bg-slate-50 dark:bg-slate-800/50 text-slate-500 dark:text-slate-400 text-xs uppercase tracking-wider">
                <th className="px-6 py-4 font-semibold">Student Info</th>
                <th className="px-6 py-4 font-semibold">Details</th>
                <th className="px-6 py-4 font-semibold">Status</th>
                <th className="px-6 py-4 font-semibold text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {loading ? (
                [1, 2, 3].map(i => (
                  <tr key={i} className="animate-pulse">
                    <td colSpan={4} className="px-6 py-8"><div className="h-4 bg-slate-100 dark:bg-slate-800 rounded w-full"></div></td>
                  </tr>
                ))
              ) : filteredStudents.length === 0 ? (
                <tr>
                  <td colSpan={4} className="px-6 py-12 text-center text-slate-500 dark:text-slate-400">
                    No students found matching your criteria.
                  </td>
                </tr>
              ) : (
                filteredStudents.map((student: any) => (
                  <tr key={student.id} className="hover:bg-slate-50/50 dark:hover:bg-white/[0.02] transition-colors">
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-full bg-primary-100 dark:bg-primary-900/30 text-primary-600 dark:text-primary-400 flex items-center justify-center font-bold">
                          {student.fullName[0]}
                        </div>
                        <div>
                          <p className="font-semibold text-slate-900 dark:text-white">{student.fullName}</p>
                          <p className="text-xs text-slate-500 flex items-center gap-1"><Mail size={12} /> {student.email}</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="space-y-1">
                        <p className="text-xs text-slate-600 dark:text-slate-400 flex items-center gap-1.5"><MapPin size={12} /> {student.address}</p>
                        <p className="text-xs text-slate-600 dark:text-slate-400 flex items-center gap-1.5"><Calendar size={12} /> {new Date(student.dateOfBirth).toLocaleDateString()}</p>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <span className={`inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium ${
                        student.approvalStatus === 'Approved' ? 'bg-emerald-50 text-emerald-600 dark:bg-emerald-500/10' :
                        student.approvalStatus === 'Rejected' ? 'bg-rose-50 text-rose-600 dark:bg-rose-500/10' :
                        'bg-amber-50 text-amber-600 dark:bg-amber-500/10'
                      }`}>
                        {student.approvalStatus}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right">
                      {student.approvalStatus === 'Pending' && (
                        <div className="flex justify-end gap-2">
                          <button 
                            onClick={() => handleApprove(student.id)}
                            className="p-2 bg-emerald-50 text-emerald-600 hover:bg-emerald-600 hover:text-white rounded-lg transition-all"
                            title="Approve"
                          >
                            <Check size={18} />
                          </button>
                          <button 
                            onClick={() => handleReject(student.id)}
                            className="p-2 bg-rose-50 text-rose-600 hover:bg-rose-600 hover:text-white rounded-lg transition-all"
                            title="Reject"
                          >
                            <X size={18} />
                          </button>
                        </div>
                      )}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};
