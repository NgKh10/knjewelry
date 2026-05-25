import React, { useState, useEffect } from 'react';
import Pagination from '../components/Pagination';
import { discountApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const Discounts = () => {
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(true);

    const [keyword, setKeyword] = useState('');
    const [loaiGiam, setLoaiGiam] = useState('');
    const [trangThai, setTrangThai] = useState('');
    const [sortOrder, setSortOrder] = useState('');

    const [page, setPage] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(0);
    const [totalCount, setTotalCount] = useState(0);

    const [showModal, setShowModal] = useState(false);
    const [editingItem, setEditingItem] = useState(null);
    const [formData, setFormData] = useState({
        ma_code: '',
        mo_ta: '',
        loai_giam: '%',
        gia_tri: '',
        giam_toi_da: '',
        don_hang_toi_thieu: 0,
        so_luong: '',
        ngay_bat_dau: '',
        ngay_ket_thuc: '',
        trang_thai: 1,
    });
    const [error, setError] = useState('');
    const { isAdmin } = useAuth();

    useEffect(() => {
        loadItems();
    }, [page, keyword, loaiGiam, trangThai, sortOrder]);

    const loadItems = async () => {
        setLoading(true);
        try {
            const params = { page, pageSize };
            if (keyword) params.keyword = keyword;
            if (loaiGiam) params.loaiGiam = loaiGiam;
            if (trangThai !== '') params.trangThai = trangThai;
            if (sortOrder) params.sortOrder = sortOrder;

            const response = await discountApi.getAll(params);
            setItems(response.data.items || []);
            setTotalPages(response.data.totalPages || 0);
            setTotalCount(response.data.totalCount || 0);
        } catch (error) {
            console.error('Failed to load:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = (e) => {
        e.preventDefault();
        setPage(1);
        loadItems();
    };

    const openModal = (item = null) => {
        if (item) {
            setEditingItem(item);
            setFormData({
                ma_code: item.ma_code || '',
                mo_ta: item.mo_ta || '',
                loai_giam: item.loai_giam || '%',
                gia_tri: item.gia_tri || '',
                giam_toi_da: item.giam_toi_da || '',
                don_hang_toi_thieu: item.don_hang_toi_thieu || 0,
                so_luong: item.so_luong || '',
                ngay_bat_dau: item.ngay_bat_dau ? item.ngay_bat_dau.substring(0, 10) : '',
                ngay_ket_thuc: item.ngay_ket_thuc ? item.ngay_ket_thuc.substring(0, 10) : '',
                trang_thai: item.trang_thai ?? 1,
            });
        } else {
            setEditingItem(null);
            setFormData({
                ma_code: '', mo_ta: '', loai_giam: '%', gia_tri: '', giam_toi_da: '',
                don_hang_toi_thieu: 0, so_luong: '', ngay_bat_dau: '', ngay_ket_thuc: '', trang_thai: 1,
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

        if (!formData.ma_code.trim()) {
            setError('Mã code không được để trống');
            return;
        }

        try {
            const data = {
                ma_code: formData.ma_code.trim().toUpperCase(),
                mo_ta: formData.mo_ta || null,
                loai_giam: formData.loai_giam,
                gia_tri: parseFloat(formData.gia_tri),
                giam_toi_da: formData.giam_toi_da ? parseFloat(formData.giam_toi_da) : null,
                don_hang_toi_thieu: parseFloat(formData.don_hang_toi_thieu) || 0,
                so_luong: formData.so_luong ? parseInt(formData.so_luong) : null,
                ngay_bat_dau: formData.ngay_bat_dau || null,
                ngay_ket_thuc: formData.ngay_ket_thuc || null,
                trang_thai: parseInt(formData.trang_thai),
            };

            if (editingItem) {
                await discountApi.update(editingItem.id_ma_giam_gia, data);
            } else {
                await discountApi.create(data);
            }

            closeModal();
            loadItems();
        } catch (error) {
            setError(error.response?.data?.message || 'Thao tác thất bại');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Bạn có chắc muốn xóa mã giảm giá này?')) return;
        try {
            await discountApi.delete(id);
            loadItems();
        } catch (error) {
            alert(error.response?.data?.message || 'Không thể xóa mã giảm giá');
        }
    };

    const formatDiscount = (item) => {
        if (item.loai_giam === '%') return `${item.gia_tri}%`;
        return `${Number(item.gia_tri).toLocaleString('vi-VN')}đ`;
    };

    const formatDate = (dateStr) => {
        if (!dateStr) return '-';
        return new Date(dateStr).toLocaleDateString('vi-VN');
    };

    return (
        <div className="content-wrapper">
            <div className="content-header">
                <div className="container-fluid">
                    <div className="row mb-2">
                        <div className="col-sm-6">
                            <h1 className="m-0">Quản lý mã giảm giá</h1>
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
                                            placeholder="Mã code, mô tả..."
                                            style={{ width: '200px' }}
                                            value={keyword}
                                            onChange={(e) => setKeyword(e.target.value)}
                                        />
                                        <select
                                            className="form-control mr-2 mb-2"
                                            value={loaiGiam}
                                            onChange={(e) => setLoaiGiam(e.target.value)}
                                        >
                                            <option value="">Tất cả loại</option>
                                            <option value="%">Phần trăm (%)</option>
                                            <option value="fixed">Tiền cố định</option>
                                        </select>
                                        <select
                                            className="form-control mr-2 mb-2"
                                            value={trangThai}
                                            onChange={(e) => setTrangThai(e.target.value)}
                                        >
                                            <option value="">Tất cả trạng thái</option>
                                            <option value="1">Hoạt động</option>
                                            <option value="0">Ngừng</option>
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
                                    {isAdmin() && (
                                        <button className="btn btn-success" onClick={() => openModal()}>
                                            <i className="fas fa-plus"></i> Thêm mã
                                        </button>
                                    )}
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
                                                <th style={{ width: '130px' }}>Mã code</th>
                                                <th>Mô tả</th>
                                                <th style={{ width: '130px' }}>Giảm giá</th>
                                                <th style={{ width: '130px' }}>ĐH tối thiểu</th>
                                                <th style={{ width: '80px' }}>Đã dùng</th>
                                                <th style={{ width: '100px' }}>Hết hạn</th>
                                                <th style={{ width: '90px' }}>Trạng thái</th>
                                                {isAdmin() && <th style={{ width: '110px' }}>Thao tác</th>}
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {items.length === 0 ? (
                                                <tr>
                                                    <td colSpan={isAdmin() ? 9 : 8} className="text-center">
                                                        Không có mã giảm giá nào
                                                    </td>
                                                </tr>
                                            ) : (
                                                items.map((item, index) => (
                                                    <tr key={item.id_ma_giam_gia}>
                                                        <td>{(page - 1) * pageSize + index + 1}</td>
                                                        <td><strong className="text-primary">{item.ma_code}</strong></td>
                                                        <td>{item.mo_ta || '-'}</td>
                                                        <td>
                                                            <span className="badge badge-warning">{formatDiscount(item)}</span>
                                                            {item.giam_toi_da && (
                                                                <small className="d-block text-muted">
                                                                    Tối đa: {Number(item.giam_toi_da).toLocaleString('vi-VN')}đ
                                                                </small>
                                                            )}
                                                        </td>
                                                        <td>
                                                            {item.don_hang_toi_thieu > 0
                                                                ? `${Number(item.don_hang_toi_thieu).toLocaleString('vi-VN')}đ`
                                                                : '-'}
                                                        </td>
                                                        <td className="text-center">
                                                            {item.da_su_dung}{item.so_luong ? `/${item.so_luong}` : ''}
                                                        </td>
                                                        <td>{formatDate(item.ngay_ket_thuc)}</td>
                                                        <td>
                                                            {item.trang_thai === 1
                                                                ? <span className="badge badge-success">Hoạt động</span>
                                                                : <span className="badge badge-secondary">Ngừng</span>
                                                            }
                                                        </td>
                                                        {isAdmin() && (
                                                            <td>
                                                                <button className="btn btn-sm btn-info mr-1" onClick={() => openModal(item)}>
                                                                    <i className="fas fa-edit"></i>
                                                                </button>
                                                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(item.id_ma_giam_gia)}>
                                                                    <i className="fas fa-trash"></i>
                                                                </button>
                                                            </td>
                                                        )}
                                                    </tr>
                                                ))
                                            )}
                                        </tbody>
                                    </table>

                                    <div className="d-flex justify-content-between align-items-center mt-3">
                                        <span>Tổng: <strong>{totalCount}</strong> mã giảm giá</span>
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
                        <div className="modal-dialog modal-lg modal-dialog-scrollable">
                            <div className="modal-content">
                                <div className="modal-header">
                                    <h5 className="modal-title">
                                        {editingItem ? 'Sửa mã giảm giá' : 'Thêm mã giảm giá mới'}
                                    </h5>
                                    <button type="button" className="close" onClick={closeModal}>&times;</button>
                                </div>
                                <form id="discountForm" onSubmit={handleSubmit} style={{ display: 'contents' }}>
                                    <div className="modal-body" style={{ overflowY: 'auto' }}>
                                        {error && <div className="alert alert-danger">{error}</div>}
                                        <div className="row">
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Mã code <span className="text-danger">*</span></label>
                                                    <input type="text" className="form-control" value={formData.ma_code}
                                                        onChange={(e) => setFormData({ ...formData, ma_code: e.target.value.toUpperCase() })}
                                                        placeholder="VD: SALE2024" required />
                                                </div>
                                            </div>
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Loại giảm <span className="text-danger">*</span></label>
                                                    <select className="form-control" value={formData.loai_giam}
                                                        onChange={(e) => setFormData({ ...formData, loai_giam: e.target.value, giam_toi_da: '' })}>
                                                        <option value="%">Phần trăm (%)</option>
                                                        <option value="fixed">Tiền cố định (đ)</option>
                                                    </select>
                                                </div>
                                            </div>
                                        </div>
                                        <div className="form-group">
                                            <label>Mô tả</label>
                                            <input type="text" className="form-control" value={formData.mo_ta}
                                                onChange={(e) => setFormData({ ...formData, mo_ta: e.target.value })}
                                                placeholder="Mô tả ngắn về mã giảm giá..." />
                                        </div>
                                        <div className="row">
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Giá trị giảm <span className="text-danger">*</span></label>
                                                    <input type="number" className="form-control" value={formData.gia_tri}
                                                        onChange={(e) => setFormData({ ...formData, gia_tri: e.target.value })}
                                                        placeholder={formData.loai_giam === '%' ? 'VD: 10' : 'VD: 50000'}
                                                        min="0" max={formData.loai_giam === '%' ? 100 : undefined}
                                                        step={formData.loai_giam === '%' ? 0.1 : 1000} required />
                                                </div>
                                            </div>
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>
                                                        Giảm tối đa (đ)
                                                        {formData.loai_giam !== '%' && <small className="text-muted ml-1">(N/A)</small>}
                                                    </label>
                                                    <input type="number" className="form-control" value={formData.giam_toi_da}
                                                        onChange={(e) => setFormData({ ...formData, giam_toi_da: e.target.value })}
                                                        placeholder="Không giới hạn"
                                                        disabled={formData.loai_giam !== '%'} min="0" step="1000" />
                                                </div>
                                            </div>
                                        </div>
                                        <div className="row">
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Đơn hàng tối thiểu (đ)</label>
                                                    <input type="number" className="form-control" value={formData.don_hang_toi_thieu}
                                                        onChange={(e) => setFormData({ ...formData, don_hang_toi_thieu: e.target.value })}
                                                        min="0" step="10000" />
                                                </div>
                                            </div>
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Số lượt tối đa</label>
                                                    <input type="number" className="form-control" value={formData.so_luong}
                                                        onChange={(e) => setFormData({ ...formData, so_luong: e.target.value })}
                                                        placeholder="Để trống: không giới hạn" min="1" />
                                                </div>
                                            </div>
                                        </div>
                                        <div className="row">
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Ngày bắt đầu</label>
                                                    <input type="date" className="form-control" value={formData.ngay_bat_dau}
                                                        onChange={(e) => setFormData({ ...formData, ngay_bat_dau: e.target.value })} />
                                                </div>
                                            </div>
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label>Ngày kết thúc</label>
                                                    <input type="date" className="form-control" value={formData.ngay_ket_thuc}
                                                        onChange={(e) => setFormData({ ...formData, ngay_ket_thuc: e.target.value })} />
                                                </div>
                                            </div>
                                        </div>
                                        {editingItem && (
                                            <div className="form-group">
                                                <label>Trạng thái</label>
                                                <select className="form-control" value={formData.trang_thai}
                                                    onChange={(e) => setFormData({ ...formData, trang_thai: e.target.value })}>
                                                    <option value="1">Hoạt động</option>
                                                    <option value="0">Ngừng</option>
                                                </select>
                                            </div>
                                        )}
                                    </div>
                                </form>
                                <div className="modal-footer">
                                    <button type="button" className="btn btn-secondary" onClick={closeModal}>Hủy</button>
                                    <button type="submit" form="discountForm" className="btn btn-primary">
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

export default Discounts;
