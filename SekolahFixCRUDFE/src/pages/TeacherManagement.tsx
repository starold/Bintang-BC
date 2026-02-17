import { useEffect, useState } from 'react';
import { adminService } from '../services/admin.service';
import type { TeacherDto } from '../types';
import { toast } from 'react-hot-toast';
import { GraduationCap, Mail, Trash2, Edit2, Plus } from 'lucide-react';

export const TeacherManagement = () => {
  const [teachers, setTeachers] = useState<TeacherDto[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchTeachers = async () => {
    setLoading(true);
    try {
      const result = await adminService.getTeachers();
      if (result.success) setTeachers(result.data.items);
    } catch (error) {
      toast.error('Failed to load teachers');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTeachers();
  }, []);

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 dark:text-white">Teacher Management</h1>
          <p className="text-slate-500 dark:text-slate-400">Manage faculty and department staff</p>
        </div>
        <button className="flex items-center gap-2 bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2.5 rounded-xl font-semibold shadow-lg shadow-indigo-500/20 transition-all active:scale-95">
          <Plus size={20} />
          Add Teacher
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {loading ? (
          [1, 2, 3].map(i => <div key={i} className="h-40 bg-slate-100 dark:bg-slate-800 rounded-2xl animate-pulse"></div>)
        ) : teachers.map(teacher => (
          <div key={teacher.id} className="bg-white dark:bg-slate-900 border border-slate-100 dark:border-slate-800 rounded-2xl p-6 shadow-sm hover:shadow-md transition-all group">
            <div className="flex items-start gap-4">
              <div className="p-3 bg-indigo-50 dark:bg-indigo-500/10 text-indigo-600 dark:text-indigo-400 rounded-xl">
                <GraduationCap size={24} />
              </div>
              <div className="flex-1 min-w-0">
                <h3 className="text-lg font-bold text-slate-900 dark:text-white truncate">{teacher.fullName}</h3>
                <p className="text-xs font-semibold text-slate-500 uppercase tracking-tighter mb-2">{teacher.department}</p>
                <div className="space-y-1">
                  <p className="text-xs text-slate-500 dark:text-slate-500 flex items-center gap-1.5 truncate">
                    <Mail size={14} className="shrink-0" /> {teacher.email}
                  </p>
                </div>
              </div>
            </div>
            
            <div className="mt-4 pt-4 border-t border-slate-50 dark:border-slate-800 flex justify-end gap-2">
              <button className="p-2 text-slate-400 hover:text-indigo-600 transition-colors"><Edit2 size={16} /></button>
              <button className="p-2 text-slate-400 hover:text-rose-600 transition-colors"><Trash2 size={16} /></button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
