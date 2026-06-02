import axios from 'axios';

<<<<<<< Updated upstream
const API_BASE_URL = '/api';
=======
// Khi VITE_API_URL trống: dùng relative URL '/api' để Vite proxy định tuyến đúng port
// Khi VITE_API_URL có giá trị (production): dùng URL tuyệt đối
const API_BASE_URL = import.meta.env.VITE_API_URL
    ? `${import.meta.env.VITE_API_URL}/api`
    : '/api';
>>>>>>> Stashed changes

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export const authApi = {
    login: (data) => api.post('/auth/login', data),
    register: (data) => api.post('/auth/register', data),
};

export const userApi = {
    getAll: (params) => api.get('/nguoidung', { params }),
    getById: (id) => api.get(`/nguoidung/${id}`),
    getMe: () => api.get('/nguoidung/me'),
    create: (data) => api.post('/nguoidung', data),
    update: (id, data) => api.put(`/nguoidung/${id}`, data),
    delete: (id) => api.delete(`/nguoidung/${id}`),
};

export const productApi = {
    getAll: (params) => api.get('/sanpham', { params }),
    getById: (id) => api.get(`/sanpham/${id}`),
    create: (data) => api.post('/sanpham', data),
    update: (id, data) => api.put(`/sanpham/${id}`, data),
    delete: (id) => api.delete(`/sanpham/${id}`),
};

export const categoryApi = {
    getAll: (params) => api.get('/loaisanpham', { params }),
    getHierarchy: () => api.get('/loaisanpham/hierarchy'),
    getParents: () => api.get('/loaisanpham/parents'),
    getById: (id) => api.get(`/loaisanpham/${id}`),
    getChildren: (id) => api.get(`/loaisanpham/${id}/children`),
    create: (data) => api.post('/loaisanpham', data),
    update: (id, data) => api.put(`/loaisanpham/${id}`, data),
    delete: (id) => api.delete(`/loaisanpham/${id}`),
};

export const materialApi = {
    getAll: (params) => api.get('/chatlieu', { params }),
    getById: (id) => api.get(`/chatlieu/${id}`),
    create: (data) => api.post('/chatlieu', data),
    update: (id, data) => api.put(`/chatlieu/${id}`, data),
    delete: (id) => api.delete(`/chatlieu/${id}`),
};

export const variantApi = {
    getAll: (params) => api.get('/bienthe', { params }),
    getById: (id) => api.get(`/bienthe/${id}`),
    getByProduct: (productId) => api.get(`/bienthe/sanpham/${productId}`),
    create: (data) => api.post('/bienthe', data),
    update: (id, data) => api.put(`/bienthe/${id}`, data),
    delete: (id) => api.delete(`/bienthe/${id}`),
};

export const productImageApi = {
    getAll: (params) => api.get('/hinhanhsanpham', { params }),
    getById: (id) => api.get(`/hinhanhsanpham/${id}`),
    getByProduct: (productId) => api.get(`/hinhanhsanpham/sanpham/${productId}`),
    create: (data) => api.post('/hinhanhsanpham', data),
    update: (id, data) => api.put(`/hinhanhsanpham/${id}`, data),
    setMain: (id) => api.put(`/hinhanhsanpham/${id}/setmain`),
    delete: (id) => api.delete(`/hinhanhsanpham/${id}`),
};

export const uploadApi = {
    uploadImage: (file) => {
        const formData = new FormData();
        formData.append('file', file);
        return api.post('/upload/image', formData, {
            headers: { 'Content-Type': 'multipart/form-data' },
        });
    },
};

export const discountApi = {
    getAll: (params) => api.get('/magiamgia', { params }),
    getById: (id) => api.get(`/magiamgia/${id}`),
    check: (code, orderTotal) => api.get(`/magiamgia/check/${code}`, { params: { orderTotal } }),
    create: (data) => api.post('/magiamgia', data),
    update: (id, data) => api.put(`/magiamgia/${id}`, data),
    delete: (id) => api.delete(`/magiamgia/${id}`),
};

export const orderApi = {
    getAll: (params) => api.get('/hoadon/all', { params }),
    getMyOrders: (params) => api.get('/hoadon', { params }),
    getById: (id) => api.get(`/hoadon/${id}`),
    create: (data) => api.post('/hoadon', data),
    updateStatus: (id, data) => api.put(`/hoadon/${id}/status`, data),
};

export const dashboardApi = {
    getRevenueByMonth: () => api.get('/thongke/doanh-thu'),
    getTopProducts: () => api.get('/thongke/top-san-pham'),
};

export default api;
