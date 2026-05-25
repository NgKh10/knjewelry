import React, { useState, useEffect } from 'react';
import Pagination from '../components/Pagination';
import { orderApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const VALID_TRANSITIONS = {
    'Chờ xác nhận': ['Đã xác nhận', 'Hủy'],
    'Đã xác nhận': ['Đang giao', 'Hủy'],
    'Đang giao': ['Hoàn thành', 'Hủy'],
    'Hoàn thành': [],
    'Hủy': [],
};

const STATUS_BADGES = {
    'Chờ xác nhận': 'badge-warning',
    'Đã xác nhận': 'badge-primary',
    'Đang giao': 'badge-info',
    'Hoàn thành': 'badge-success',
    'Hủy': 'badge-danger',
};

const STATUS_BUTTON_LABELS = {
    'Đã xác nhận': 'Xác nhận',
    'Đang giao': 'Đang giao hàng',
    'Hoàn thành': 'Giao hàng thành công',
    'Hủy': 'Hủy đơn',
};

const Orders = () => {
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(true);

    const [keyword, setKeyword] = useState('');
    const [trangThai, setTrangThai] = useState('');
    const [tuNgay, setTuNgay] = useState('');
    const [denNgay, setDenNgay] = useState('');
    const [sortOrder, setSortOrder] = useState('');

    const [page, setPage] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(0);
    const [totalCount, setTotalCount] = useState(0);

    const [showDetail, setShowDetail] = useState(false);
    const [selectedOrder, setSelectedOrder] = useState(null);
    const [loadingDetail, setLoadingDetail] = useState(false);
    const [statusUpdating, setStatusUpdating] = useState(false);
    const [statusError, setStatusError] = useState('');
    const { isAdmin } = useAuth();

    useEffect(() => {
        loadItems();
    }, [page, keyword, trangThai, tuNgay, denNgay, sortOrder]);

    const loadItems = async () => {
        setLoading(true);
        try {
            const params = { page, pageSize };
            if (keyword) params.keyword = keyword;
            if (trangThai) params.trangThai = trangThai;
            if (tuNgay) params.tuNgay = tuNgay;
            if (denNgay) params.denNgay = denNgay;
            if (sortOrder) params.sortOrder = sortOrder;

            const response = await orderApi.getAll(params);
            setItems(response.data.items || []);
            setTotalPages(response.data.totalPages || 0);
            setTotalCount(response.data.totalCount || 0);
            setStatusError('');
        } catch (error) {
            console.error('Failed to load orders:', error);
            if (error.response?.status === 403) {
                setStatusError('Bạn không có quyền truy cập trang này. Vui lòng đăng nhập lại với tài khoản quản trị.');
            }
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = (e) => {
        e.preventDefault();
        setPage(1);
        loadItems();
    };

    const viewDetail = async (id) => {
        setLoadingDetail(true);
        setShowDetail(true);
        setSelectedOrder(null);
        try {
            const response = await orderApi.getById(id);
            setSelectedOrder(response.data);
        } catch (error) {
            alert('Không thể tải chi tiết đơn hàng');
            setShowDetail(false);
        } finally {
            setLoadingDetail(false);
        }
    };

    const handleUpdateStatus = async (id, newStatus) => {
        if (!window.confirm(`Chuyển trạng thái sang "${newStatus}"?`)) return;
        setStatusUpdating(true);
        setStatusError('');
        try {
            await orderApi.updateStatus(id, { trang_thai: newStatus });
            await loadItems();
            if (selectedOrder?.id_hoa_don === id) {
                const response = await orderApi.getById(id);
                setSelectedOrder(response.data);
            }
        } catch (error) {
            const msg = error.response?.data?.message || `Lỗi ${error.response?.status || ''}: Không thể cập nhật trạng thái`;
            setStatusError(msg);
        } finally {
            setStatusUpdating(false);
        }
    };

    const formatDate = (dateStr) => {
        if (!dateStr) return '-';
        return new Date(dateStr).toLocaleString('vi-VN');
    };

    const getNextStatuses = (currentStatus) => VALID_TRANSITIONS[currentStatus] || [];

    const getStatusButtonClass = (status) => {
        if (status === 'Hủy') return 'btn-danger';
        if (status === 'Hoàn thành') return 'btn-success';
        if (status === 'Đang giao') return 'btn-info';
        if (status === 'Đã xác nhận') return 'btn-primary';
        return 'btn-warning';
    };

    return (
        <div className="content-wrapper">
            <div className="content-header">
                <div className="container-fluid">
                    <div className="row mb-2">
                        <div className="col-sm-6">
                            <h1 className="m-0">Quản lý đơn hàng</h1>
                        </div>
                    </div>
                </div>
            </div>

            <section className="content">
                <div className="container-fluid">
                    <div className="card">
                        <div className="card-header">
                            <form onSubmit={handleSearch} className="form-inline flex-wrap">
                                <input
                                    type="text"
                                    className="form-control mr-2 mb-2"
                                    placeholder="Mã HĐ, tên, email, SĐT..."
                                    style={{ width: '220px' }}
                                    value={keyword}
                                    onChange={(e) => setKeyword(e.target.value)}
                                />
                                <select
                                    className="form-control mr-2 mb-2"
                                    value={trangThai}
                                    onChange={(e) => setTrangThai(e.target.value)}
                                >
                                    <option value="">Tất cả trạng thái</option>
                                    <option value="Chờ xác nhận">Chờ xác nhận</option>
                                    <option value="Đã xác nhận">Đã xác nhận</option>
                                    <option value="Đang giao">Đang giao</option>
                                    <option value="Hoàn thành">Hoàn thành</option>
                                    <option value="Hủy">Hủy</option>
                                </select>
                                <input
                                    type="date"
                                    className="form-control mr-2 mb-2"
                                    value={tuNgay}
                                    onChange={(e) => setTuNgay(e.target.value)}
                                    title="Từ ngày"
                                />
                                <input
                                    type="date"
                                    className="form-control mr-2 mb-2"
                                    value={denNgay}
                                    onChange={(e) => setDenNgay(e.target.value)}
                                    title="Đến ngày"
                                />
                                <select className="form-control mr-2 mb-2" value={sortOrder} onChange={e => { setSortOrder(e.target.value); setPage(1); }}>
                                    <option value="">Mặc định</option>
                                    <option value="asc">Tên KH: A → Z</option>
                                    <option value="desc">Tên KH: Z → A</option>
                                </select>
                                <button type="submit" className="btn btn-primary mb-2">
                                    <i className="fas fa-search"></i> Tìm
                                </button>
                            </form>
                        </div>
                        <div className="card-body">
                            {statusError && (
                                <div className="alert alert-danger alert-dismissible">
                                    <button type="button" className="close" onClick={() => setStatusError('')}>&times;</button>
                                    <i className="fas fa-exclamation-triangle mr-1"></i> {statusError}
                                </div>
                            )}
                            {loading ? (
                                <div className="text-center py-5">
                                    <div className="spinner-border text-primary"></div>
                                </div>
                            ) : (
                                <>
                                    <table className="table table-bordered table-striped">
                                        <thead>
                                            <tr>
                                                <th style={{ width: '50px' }}>STT</th>
                                                <th style={{ width: '130px' }}>Mã HĐ</th>
                                                <th>Khách hàng</th>
                                                <th style={{ width: '120px' }}>Tổng tiền</th>
                                                <th style={{ width: '140px' }}>Trạng thái</th>
                                                <th style={{ width: '125px' }}>Ngày đặt</th>
                                                <th style={{ width: '240px' }}>Thao tác</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {items.length === 0 ? (
                                                <tr>
                                                    <td colSpan="7" className="text-center">Không có đơn hàng nào</td>
                                                </tr>
                                            ) : (
                                                items.map((item, index) => (
                                                    <tr key={item.id_hoa_don}>
                                                        <td>{(page - 1) * pageSize + index + 1}</td>
                                                        <td><strong>{item.ma_hoa_don}</strong></td>
                                                        <td>
                                                            <div>{item.ho_ten}</div>
                                                            <small className="text-muted">{item.so_dien_thoai}</small>
                                                        </td>
                                                        <td><strong>{Number(item.tong_tien).toLocaleString('vi-VN')}đ</strong></td>
                                                        <td>
                                                            <span className={`badge ${STATUS_BADGES[item.trang_thai] || 'badge-secondary'}`}>
                                                                {item.trang_thai}
                                                            </span>
                                                        </td>
                                                        <td><small>{formatDate(item.thoi_gian_dat)}</small></td>
                                                        <td>
                                                            <button
                                                                className="btn btn-sm btn-secondary mr-1"
                                                                onClick={() => viewDetail(item.id_hoa_don)}
                                                                title="Xem chi tiết"
                                                            >
                                                                <i className="fas fa-eye"></i>
                                                            </button>
                                                            {isAdmin() && getNextStatuses(item.trang_thai).map(status => (
                                                                <button key={status}
                                                                    className={`btn btn-sm ${getStatusButtonClass(status)} mr-1 mb-1`}
                                                                    onClick={() => handleUpdateStatus(item.id_hoa_don, status)}
                                                                    disabled={statusUpdating}
                                                                >
                                                                    {STATUS_BUTTON_LABELS[status] || status}
                                                                </button>
                                                            ))}
                                                        </td>
                                                    </tr>
                                                ))
                                            )}
                                        </tbody>
                                    </table>

                                    <div className="d-flex justify-content-between align-items-center mt-3">
                                        <span>Tổng: <strong>{totalCount}</strong> đơn hàng</span>
                                        <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
                                    </div>
                                </>
                            )}
                        </div>
                    </div>
                </div>
            </section>

            {showDetail && (
                <>
                    <div className="modal fade show" style={{ display: 'block' }} tabIndex="-1">
                        <div className="modal-dialog modal-lg modal-dialog-scrollable" style={{ maxHeight: '90vh' }}>
                            <div className="modal-content">
                                <div className="modal-header">
                                    <h5 className="modal-title">
                                        Chi tiết đơn hàng {selectedOrder?.ma_hoa_don}
                                    </h5>
                                    <button type="button" className="close" onClick={() => setShowDetail(false)}>&times;</button>
                                </div>
                                <div className="modal-body" style={{ overflowY: 'auto' }}>
                                    {loadingDetail ? (
                                        <div className="text-center py-3">
                                            <div className="spinner-border text-primary"></div>
                                        </div>
                                    ) : selectedOrder && (
                                        <>
                                            <div className="row">
                                                <div className="col-md-6">
                                                    <h6 className="text-muted mb-2">Thông tin khách hàng</h6>
                                                    <p className="mb-1"><strong>Họ tên:</strong> {selectedOrder.ho_ten}</p>
                                                    <p className="mb-1"><strong>SĐT:</strong> {selectedOrder.so_dien_thoai}</p>
                                                    <p className="mb-1"><strong>Email:</strong> {selectedOrder.email}</p>
                                                    <p className="mb-1">
                                                        <strong>Địa chỉ:</strong> {selectedOrder.dia_chi_cu_the}, {selectedOrder.phuong_xa}, {selectedOrder.tinh_thanh_pho}
                                                    </p>
                                                </div>
                                                <div className="col-md-6">
                                                    <h6 className="text-muted mb-2">Thông tin đơn hàng</h6>
                                                    <p className="mb-1">
                                                        <strong>Trạng thái:</strong>{' '}
                                                        <span className={`badge ${STATUS_BADGES[selectedOrder.trang_thai] || 'badge-secondary'}`}>
                                                            {selectedOrder.trang_thai}
                                                        </span>
                                                    </p>
                                                    <p className="mb-1"><strong>Ngày đặt:</strong> {formatDate(selectedOrder.thoi_gian_dat)}</p>
                                                    <p className="mb-1"><strong>PT thanh toán:</strong> {selectedOrder.phuong_thuc_tt}</p>
                                                    {selectedOrder.ghi_chu && (
                                                        <p className="mb-1"><strong>Ghi chú:</strong> {selectedOrder.ghi_chu}</p>
                                                    )}
                                                </div>
                                            </div>
                                            <hr />
                                            <h6 className="text-muted mb-2">Sản phẩm</h6>
                                            {selectedOrder.chiTietHoaDons?.length > 0 ? (
                                                <table className="table table-sm table-bordered">
                                                    <thead className="thead-light">
                                                        <tr>
                                                            <th>Sản phẩm</th>
                                                            <th style={{ width: '60px' }} className="text-center">SL</th>
                                                            <th style={{ width: '110px' }}>Đơn giá</th>
                                                            <th style={{ width: '110px' }}>Thành tiền</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        {selectedOrder.chiTietHoaDons.map((ct, idx) => (
                                                            <tr key={idx}>
                                                                <td>
                                                                    <div>{ct.ten_sp_luu}</div>
                                                                    {(ct.mau_sac_luu || ct.kich_co_luu) && (
                                                                        <small className="text-muted">
                                                                            {[ct.kich_co_luu, ct.mau_sac_luu].filter(Boolean).join(' / ')}
                                                                        </small>
                                                                    )}
                                                                </td>
                                                                <td className="text-center">{ct.so_luong}</td>
                                                                <td>{Number(ct.don_gia).toLocaleString('vi-VN')}đ</td>
                                                                <td>{Number(ct.thanh_tien || ct.so_luong * ct.don_gia).toLocaleString('vi-VN')}đ</td>
                                                            </tr>
                                                        ))}
                                                    </tbody>
                                                </table>
                                            ) : (
                                                <p className="text-muted">Không có chi tiết sản phẩm</p>
                                            )}
                                            <div className="text-right mt-2">
                                                <p className="mb-1">Tiền hàng: <strong>{Number(selectedOrder.tien_hang).toLocaleString('vi-VN')}đ</strong></p>
                                                {selectedOrder.tien_giam > 0 && (
                                                    <p className="mb-1 text-success">
                                                        Giảm giá: -<strong>{Number(selectedOrder.tien_giam).toLocaleString('vi-VN')}đ</strong>
                                                    </p>
                                                )}
                                                <p className="mb-1">Phí vận chuyển: <strong>{Number(selectedOrder.phi_van_chuyen).toLocaleString('vi-VN')}đ</strong></p>
                                                <h5 className="mt-2">
                                                    Tổng cộng: <strong className="text-primary">{Number(selectedOrder.tong_tien).toLocaleString('vi-VN')}đ</strong>
                                                </h5>
                                            </div>
                                        </>
                                    )}
                                </div>
                                {isAdmin() && selectedOrder && getNextStatuses(selectedOrder.trang_thai).length > 0 && (
                                    <div className="modal-footer justify-content-between">
                                        <div>
                                            <strong className="mr-2">Chuyển trạng thái:</strong>
                                            {statusUpdating && <span className="spinner-border spinner-border-sm mr-2 text-primary"></span>}
                                            {getNextStatuses(selectedOrder.trang_thai).map(status => (
                                                <button key={status}
                                                    className={`btn btn-sm ${getStatusButtonClass(status)} mr-1`}
                                                    onClick={() => handleUpdateStatus(selectedOrder.id_hoa_don, status)}
                                                    disabled={statusUpdating}
                                                >
                                                    {STATUS_BUTTON_LABELS[status] || status}
                                                </button>
                                            ))}
                                        </div>
                                        <button type="button" className="btn btn-secondary" onClick={() => setShowDetail(false)}>Đóng</button>
                                    </div>
                                )}
                                {(!isAdmin() || !selectedOrder || getNextStatuses(selectedOrder.trang_thai).length === 0) && (
                                    <div className="modal-footer">
                                        <button type="button" className="btn btn-secondary" onClick={() => setShowDetail(false)}>Đóng</button>
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                    <div className="modal-backdrop fade show"></div>
                </>
            )}
        </div>
    );
};

export default Orders;
