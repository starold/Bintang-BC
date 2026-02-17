import { useEffect, useState } from 'react';
import { adminService } from '../services/admin.service';
import type { CourseDto, TeacherDto } from '../types';
import { toast } from 'react-hot-toast';
import { Plus, Edit2, Trash2, BookOpen } from 'lucide-react';
import { Modal } from '../components/Modal';
import { useForm } from 'react-hook-form';

export const CourseManagement = () => {
  const [courses, setCourses] = useState<CourseDto[]>([]);
  const [teachers, setTeachers] = useState<TeacherDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingCourse, setEditingCourse] = useState<CourseDto | null>(null);
  
  const { register, handleSubmit, reset } = useForm();

  const fetchData = async () => {
    setLoading(true);
    try {
      const [courseRes, teacherRes] = await Promise.all([
        adminService.getCourses(),
        adminService.getTeachers()
      ]);
      if (courseRes.success) setCourses(courseRes.data.items);
      if (teacherRes.success) setTeachers(teacherRes.data.items);
    } catch (error) {
      toast.error('Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const onSubmit = async (data: any) => {
    try {
      if (editingCourse) {
        await adminService.updateCourse(editingCourse.id, data);
        toast.success('Course updated');
      } else {
        await adminService.createCourse(data);
        toast.success('Course created');
      }
      setIsModalOpen(false);
      fetchData();
    } catch (error) {
      toast.error('Operation failed');
    }
  };

  const openAddModal = () => {
    setEditingCourse(null);
    reset({});
    setIsModalOpen(true);
  };

  const openEditModal = (course: CourseDto) => {
    setEditingCourse(course);
    reset({
      name: course.name,
      code: course.code,
      credits: course.credits,
      description: course.description,
      teacherId: course.teacherId
    });
    setIsModalOpen(true);
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this course?')) return;
    try {
      await adminService.deleteCourse(id);
      toast.success('Course deleted');
      fetchData();
    } catch (error) {
      toast.error('Deletion failed');
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 dark:text-white">Course Management</h1>
          <p className="text-slate-500 dark:text-slate-400">Add and manage curriculum courses</p>
        </div>
        <button 
          onClick={openAddModal}
          className="flex items-center gap-2 bg-primary-600 hover:bg-primary-700 text-white px-4 py-2.5 rounded-xl font-semibold shadow-lg shadow-primary-500/20 transition-all active:scale-95"
        >
          <Plus size={20} />
          Add Course
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {loading ? (
          [1, 2, 3].map(i => <div key={i} className="h-48 bg-slate-100 dark:bg-slate-800 rounded-2xl animate-pulse"></div>)
        ) : courses.map(course => (
          <div key={course.id} className="bg-white dark:bg-slate-900 border border-slate-100 dark:border-slate-800 rounded-2xl p-6 shadow-sm hover:shadow-md transition-all group">
            <div className="flex justify-between items-start mb-4">
              <div className="p-3 bg-primary-50 dark:bg-primary-500/10 text-primary-600 dark:text-primary-400 rounded-xl">
                <BookOpen size={24} />
              </div>
              <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                <button onClick={() => openEditModal(course)} className="p-2 text-slate-400 hover:text-primary-600 transition-colors"><Edit2 size={18} /></button>
                <button onClick={() => handleDelete(course.id)} className="p-2 text-slate-400 hover:text-rose-600 transition-colors"><Trash2 size={18} /></button>
              </div>
            </div>
            
            <h3 className="text-lg font-bold text-slate-900 dark:text-white mb-1">{course.name}</h3>
            <p className="text-xs font-mono text-primary-600 dark:text-primary-400 mb-3">{course.code} • {course.credits} Credits</p>
            <p className="text-sm text-slate-500 dark:text-slate-400 line-clamp-2 mb-4">
              {course.description || 'No description provided for this course.'}
            </p>
            
            <div className="pt-4 border-t border-slate-50 dark:border-slate-800 flex items-center gap-2">
              <div className="w-6 h-6 rounded-full bg-slate-100 dark:bg-slate-800 flex items-center justify-center text-[10px] font-bold text-slate-500">
                {course.teacherName?.[0] || '?'}
              </div>
              <p className="text-xs text-slate-600 dark:text-slate-400">
                {course.teacherName || 'No teacher assigned'}
              </p>
            </div>
          </div>
        ))}
      </div>

      <Modal 
        isOpen={isModalOpen} 
        onClose={() => setIsModalOpen(false)} 
        title={editingCourse ? 'Edit Course' : 'Add New Course'}
      >
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="col-span-2 space-y-1.5">
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider ml-1">Course Name</label>
              <input {...register('name', { required: true })} className="w-full bg-slate-50 dark:bg-slate-800 border-none rounded-xl p-3 text-sm focus:ring-2 focus:ring-primary-500/20 outline-none" placeholder="Web Development" />
            </div>
            <div className="space-y-1.5">
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider ml-1">Code</label>
              <input {...register('code', { required: true })} className="w-full bg-slate-50 dark:bg-slate-800 border-none rounded-xl p-3 text-sm focus:ring-2 focus:ring-primary-500/20 outline-none" placeholder="CS101" />
            </div>
            <div className="space-y-1.5">
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider ml-1">Credits</label>
              <input type="number" {...register('credits', { required: true })} className="w-full bg-slate-50 dark:bg-slate-800 border-none rounded-xl p-3 text-sm focus:ring-2 focus:ring-primary-500/20 outline-none" placeholder="3" />
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider ml-1">Assign Teacher</label>
            <select {...register('teacherId')} className="w-full bg-slate-50 dark:bg-slate-800 border-none rounded-xl p-3 text-sm focus:ring-2 focus:ring-primary-500/20 outline-none appearance-none">
              <option value="">Select a teacher</option>
              {teachers.map(t => <option key={t.id} value={t.id}>{t.fullName}</option>)}
            </select>
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider ml-1">Description</label>
            <textarea {...register('description')} rows={3} className="w-full bg-slate-50 dark:bg-slate-800 border-none rounded-xl p-3 text-sm focus:ring-2 focus:ring-primary-500/20 outline-none resize-none" placeholder="Course overview..." />
          </div>

          <div className="flex gap-3 mt-6">
            <button type="button" onClick={() => setIsModalOpen(false)} className="flex-1 px-4 py-3 rounded-xl border border-slate-200 dark:border-slate-800 font-semibold text-slate-600 hover:bg-slate-50 transition-all">Cancel</button>
            <button type="submit" className="flex-1 px-4 py-3 rounded-xl bg-primary-600 text-white font-semibold shadow-lg shadow-primary-500/20 hover:bg-primary-700 transition-all">
              {editingCourse ? 'Save Changes' : 'Create Course'}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};
