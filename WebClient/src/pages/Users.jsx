import React, { useState, useEffect } from 'react';
import Pagination from '../components/Pagination';
import { userApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const Users = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);

    const [keyword, setKeyword] = useState('');
    const [vaiTro, setVaiTro] = useState('');
    const [trangThai, setTrangThai] = useState('');
    const [sortOrder, setSortOrder] = useState('');

    const [page, setPage] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(0);
    const [totalCount, setTotalCount] = useState(0);

    const [showModal, setShowModal] = useState(false);
    const [editingItem, setEditingItem] = useState(null);
    const [formData, setFormData] = useState({
        ten_dang_nhap: '',
        mat_khau: '',
        ho_ten: '',
        email: '',
        so_dien_thoai: '',
        dia_chi: '',
        vai_tro: 'khach_hang',
        trang_thai: 1,
    });
    const [error, setError] = useState('');
    const { isAdmin } = useAuth();

    useEffect(() => {
        loadUsers();
    }, [page, keyword, vaiTro, trangThai, sortOrder]);

    const loadUsers = async () => {
        setLoading(true);
        try {
            const params = { page, pageSize };
            if (keyword) params.keyword = keyword;
            if (vaiTro) params.vaiTro = vaiTro;
            if (trangThai !== '') params.trangThai = trangThai;
            if (sortOrder) params.sortOrder = sortOrder;

            const response = await userApi.getAll(params);
            setUsers(response.data.items || []);
            setTotalPages(response.data.totalPages || 0);
            setTotalCount(response.data.totalCount || 0);
        } catch (error) {
            console.error('Failed to load users:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = (e) => {
        e.preventDefault();
        setPage(1);
        loadUsers();
    };

    const openModal = (item = null) => {
        if (item) {
            setEditingItem(item);
            setFormData({
                ten_dang_nhap: item.ten_dang_nhap || '',
                mat_khau: '',
                ho_ten: item.ho_ten || '',
                email: item.email || '',
                so_dien_thoai: item.so_dien_thoai || '',
                dia_chi: item.dia_chi || '',
                vai_tro: item.vai_tro || 'khach_hang',
                trang_thai: item.trang_thai ?? 1,
            });
        } else {
            setEditingItem(null);
            setFormData({
                ten_dang_nhap: '',
                mat_khau: '',
                ho_ten: '',
                email: '',
                so_dien_thoai: '',
                dia_chi: '',
                vai_tro: 'khach_hang',
                trang_thai: 1,
            });
        }
        setError('');
        setShowModal(true);
    };

    const closeModal = () => {
        setShowModal(false);
        setEditingItem(null);
        setError('');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            if (editingItem) {
                const data = {
                    ho_ten: formData.ho_ten,
                    email: formData.email,
                    so_dien_thoai: formData.so_dien_thoai,
                    dia_chi: formData.dia_chi,
                    vai_tro: formData.vai_tro,
                    trang_thai: parseInt(formData.trang_thai),
                };
                if (formData.mat_khau) data.mat_khau = formData.mat_khau;
                await userApi.update(editingItem.id_nguoi_dung, data);
            } else {
                if (!formData.mat_khau) {
                    setError('Mật khẩu không được để trống');
                    return;
                }
                await userApi.create({
                    ten_dang_nhap: formData.ten_dang_nhap,
                    mat_khau: formData.mat_khau,
                    ho_ten: formData.ho_ten,
                    email: formData.email,
                    so_dien_thoai: formData.so_dien_thoai,
                    dia_chi: formData.dia_chi,
                    vai_tro: formData.vai_tro,
                });
            }
            closeModal();
            loadUsers();
        } catch (error) {
            setError(error.response?.data?.message || 'Thao tác thất bại');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Bạn có chắc muốn xóa người dùng này?')) return;
        try {
            await userApi.delete(id);
            loadUsers();
        } catch (error) {
            alert(error.response?.data?.message || 'Không thể xóa người dùng');
        }
    };

    return (
        <div className="content-wrapper">
            <div className="content-header">
                <div className="container-fluid">
                    <div className="row mb-2">
                        <div className="col-sm-6">
                            <h1 className="m-0">Quản lý người dùng</h1>
                        </div>
                    </div>
                </div>
            </div>

            <section className="content">
                <div className="container-fluid">
                    <div className="card">
                        <div className="card-header">
                            <div className="row">
                                <div className="col-md-9">
                                    <form onSubmit={handleSearch} className="form-inline">
                                        <input
                                            type="text"
                                            className="form-control mr-2 mb-2"
                                            placeholder="Tên, email, số điện thoại..."
                                            style={{ width: '220px' }}
                                            value={keyword}
                                            onChange={(e) => setKeyword(e.target.value)}
                                        />
                                        <select
                                            className="form-control mr-2 mb-2"
                                            value={vaiTro}
                                            onChange={(e) => setVaiTro(e.target.value)}
                                        >
                                            <option value="">Tất cả vai trò</option>
                                            <option value="quan_tri">Quản trị</option>
                                            <option value="khach_hang">Khách hàng</option>
                                        </select>
                                        <select
                                            className="form-control mr-2 mb-2"
                                            value={trangThai}
                                            onChange={(e) => setTrangThai(e.target.value)}
                                        >
                                            <option value="">Tất cả trạng thái</option>
                                            <option value="1">Hoạt động</option>
                                            <option value="0">Đã khóa</option>
                                        </select>
                                        <select className="form-control mr-2 mb-2" value={sortOrder} onChange={e => { setSortOrder(e.target.value); setPage(1); }}>
                                            <option value="">Mặc định</option>
                                            <option value="asc">A → Z</option>
                                            <option value="desc">Z → A</option>
                                        </select>
                                        <button type="submit" className="btn btn-primary mb-2">
                                            <i className="fas fa-search"></i> Tìm
                                        </button>
                                    </form>
                                </div>
                                <div className="col-md-3 text-right">
                                    <button className="btn btn-success" onClick={() => openModal()}>
                                        <i className="fas fa-plus"></i> Thêm người dùng
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div className="card-body">
                            {loading ? (
                                <div className="text-center py-5">
                                    <div className="spinner-border text-primary"></div>
                                </div>
                            ) : (
                                <>
                                    <table className="table table-bordered table-striped">
                                        <thead>
                                            <tr>
                                                <th style={{ width: '60px' }}>STT</th>
                                                <th>Tên đăng nhập</th>
                                                <th>Họ tên</th>
                                                <th>Email</th>
                                                <th style={{ width: '120px' }}>Số điện thoại</th>
                                                <th style={{ width: '110px' }}>Vai trò</th>
                                                <th style={{ width: '100px' }}>Trạng thái</th>
                                                <th style={{ width: '110px' }}>Thao tác</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {users.length === 0 ? (
                                                <tr>
                                                    <td colSpan="8" className="text-center">Không có người dùng nào</td>
                                                </tr>
                                            ) : (
                                                users.map((item, index) => (
                                                    <tr key={item.id_nguoi_dung}>
                                                        <td>{(page - 1) * pageSize + index + 1}</td>
                                                        <td><strong>{item.ten_dang_nhap}</strong></td>
                                                        <td>{item.ho_ten}</td>
                                                        <td>{item.email}</td>
                                                        <td>{item.so_dien_thoai || '-'}</td>
                                                        <td>
                                                            {item.vai_tro === 'quan_tri'
                                                                ? <span className="badge badge-danger">Quản trị</span>
                                                                : <span className="badge badge-info">Khách hàng</span>
                                                            }
                                                        </td>
                                                        <td>
                                                            {item.trang_thai === 1
                                                                ? <span className="badge badge-success">Hoạt động</span>
                                                                : <span className="badge badge-secondary">Đã khóa</span>
                                                            }
                                                        </td>
                                                        <td>
                                                            <button className="btn btn-sm btn-info mr-1" onClick={() => openModal(item)}>
                                                                <i className="fas fa-edit"></i>
                                                            </button>
                                                            <button className="btn btn-sm btn-danger" onClick={() => handleDelete(item.id_nguoi_dung)}>
                                                                <i className="fas fa-trash"></i>
                                                            </button>
                                                        </td>
                                                    </tr>
                                                ))
                                            )}
                                        </tbody>
                                    </table>

                                    <div className="d-flex justify-content-between align-items-center mt-3">
                                        <span>Tổng: <strong>{totalCount}</strong> người dùng</span>
                                        <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
                                    </div>
                                </>
                            )}
                        </div>
                    </div>
                </div>
            </section>

            {showModal && (
                <>
                    <div className="modal fade show" style={{ display: 'block' }} tabIndex="-1">
                        <div className="modal-dialog modal-dialog-scrollable">
                            <div className="modal-content">
                                <div className="modal-header">
                                    <h5 className="modal-title">
                                        {editingItem ? 'Sửa người dùng' : 'Thêm người dùng mới'}
                                    </h5>
                                    <button type="button" className="close" onClick={closeModal}>&times;</button>
                                </div>
                                <form id="userForm" onSubmit={handleSubmit} style={{ display: 'contents' }}>
                                    <div className="modal-body" style={{ overflowY: 'auto' }}>
                                        {error && <div className="alert alert-danger">{error}</div>}
                                        {!editingItem && (
                                            <div className="form-group">
                                                <label>Tên đăng nhập <span className="text-danger">*</span></label>
                                                <input type="text" className="form-control" value={formData.ten_dang_nhap}
                                                    onChange={(e) => setFormData({ ...formData, ten_dang_nhap: e.target.value })}
                                                    required />
                                            </div>
                                        )}
                                        <div className="form-group">
                                            <label>Mật khẩu {editingItem ? '(để trống để giữ nguyên)' : <span className="text-danger">*</span>}</label>
                                            <input type="password" className="form-control" value={formData.mat_khau}
                                                onChange={(e) => setFormData({ ...formData, mat_khau: e.target.value })}
                                                required={!editingItem} />
                                        </div>
                                        <div className="form-group">
                                            <label>Họ tên</label>
                                            <input type="text" className="form-control" value={formData.ho_ten}
                                                onChange={(e) => setFormData({ ...formData, ho_ten: e.target.value })} />
                                        </div>
                                        <div className="form-group">
                                            <label>Email</label>
                                            <input type="email" className="form-control" value={formData.email}
                                                onChange={(e) => setFormData({ ...formData, email: e.target.value })} />
                                        </div>
                                        <div className="form-group">
                                            <label>Số điện thoại</label>
                                            <input type="text" className="form-control" value={formData.so_dien_thoai}
                                                onChange={(e) => setFormData({ ...formData, so_dien_thoai: e.target.value })} />
                                        </div>
                                        <div className="form-group">
                                            <label>Địa chỉ</label>
                                            <input type="text" className="form-control" value={formData.dia_chi}
                                                onChange={(e) => setFormData({ ...formData, dia_chi: e.target.value })} />
                                        </div>
                                        <div className="form-group">
                                            <label>Vai trò</label>
                                            <select className="form-control" value={formData.vai_tro}
                                                onChange={(e) => setFormData({ ...formData, vai_tro: e.target.value })}>
                                                <option value="khach_hang">Khách hàng</option>
                                                <option value="quan_tri">Quản trị</option>
                                            </select>
                                        </div>
                                        {editingItem && (
                                            <div className="form-group">
                                                <label>Trạng thái</label>
                                                <select className="form-control" value={formData.trang_thai}
                                                    onChange={(e) => setFormData({ ...formData, trang_thai: e.target.value })}>
                                                    <option value="1">Hoạt động</option>
                                                    <option value="0">Khóa</option>
                                                </select>
                                            </div>
                                        )}
                                    </div>
                                </form>
                                <div className="modal-footer">
                                    <button type="button" className="btn btn-secondary" onClick={closeModal}>Hủy</button>
                                    <button type="submit" form="userForm" className="btn btn-primary">
                                        {editingItem ? 'Cập nhật' : 'Thêm mới'}
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="modal-backdrop fade show"></div>
                </>
            )}
        </div>
    );
};

export default Users;
