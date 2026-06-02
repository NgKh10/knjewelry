import React from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const MainLayout = ({ children }) => {
    const location = useLocation();
    const navigate = useNavigate();
    const { user, logout, isAdmin } = useAuth();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    const isActive = (path) => location.pathname === path ? 'active' : '';

    return (
        <div className="wrapper">
            <nav className="main-header navbar navbar-expand navbar-white navbar-light">
                <ul className="navbar-nav">
                    <li className="nav-item">
                        <a className="nav-link" data-widget="pushmenu" href="#" role="button">
                            <i className="fas fa-bars"></i>
                        </a>
                    </li>
                    <li className="nav-item d-none d-sm-inline-block">
                        <Link to="/" className="nav-link">Home</Link>
                    </li>
                </ul>

                <ul className="navbar-nav ml-auto">
                    <li className="nav-item dropdown">
                        <a className="nav-link" data-toggle="dropdown" href="#">
                            {/* Sửa: dùng đúng field từ API (ho_ten, ten_dang_nhap) */}
                            <i className="far fa-user"></i> {user?.ho_ten || user?.ten_dang_nhap}
                        </a>
                        <div className="dropdown-menu dropdown-menu-right">
                            <span className="dropdown-item dropdown-header">{user?.email}</span>
                            <div className="dropdown-divider"></div>
                            <button className="dropdown-item" onClick={handleLogout}>
                                <i className="fas fa-sign-out-alt mr-2"></i> Đăng xuất
                            </button>
                        </div>
                    </li>
                </ul>
            </nav>

            <aside className="main-sidebar sidebar-dark-primary elevation-4">
                <Link to="/" className="brand-link">
                    <span className="brand-text font-weight-light ml-3">
                        <b>Trang Sức</b> Bạc
                    </span>
                </Link>

                <div className="sidebar">
                    <div className="user-panel mt-3 pb-3 mb-3 d-flex">
                        <div className="image">
                            <i className="fas fa-user-circle fa-2x text-light"></i>
                        </div>
                        <div className="info">
                            {/* Sửa: dùng đúng field từ API */}
                            <Link to="#" className="d-block">{user?.ho_ten || user?.ten_dang_nhap}</Link>
                        </div>
                    </div>

                    <nav className="mt-2">
                        <ul className="nav nav-pills nav-sidebar flex-column" data-widget="treeview" role="menu">
                            <li className="nav-item">
                                <Link to="/" className={`nav-link ${isActive('/')}`}>
                                    <i className="nav-icon fas fa-tachometer-alt"></i>
                                    <p>Dashboard</p>
                                </Link>
                            </li>

                            <li className="nav-header">QUẢN LÝ SẢN PHẨM</li>

                            <li className="nav-item">
                                <Link to="/products" className={`nav-link ${isActive('/products')}`}>
                                    <i className="nav-icon fas fa-box"></i>
                                    <p>Sản phẩm</p>
                                </Link>
                            </li>
                            <li className="nav-item">
                                <Link to="/categories" className={`nav-link ${isActive('/categories')}`}>
                                    <i className="nav-icon fas fa-tags"></i>
                                    <p>Danh mục</p>
                                </Link>
                            </li>
                            <li className="nav-item">
                                <Link to="/chatlieu" className={`nav-link ${isActive('/chatlieu')}`}>
                                    <i className="nav-icon fas fa-gem"></i>
                                    <p>Chất liệu</p>
                                </Link>
                            </li>
                            <li className="nav-item">
                                <Link to="/variants" className={`nav-link ${isActive('/variants')}`}>
                                    <i className="nav-icon fas fa-layer-group"></i>
                                    <p>Biến thể</p>
                                </Link>
                            </li>
                            <li className="nav-item">
                                <Link to="/product-images" className={`nav-link ${isActive('/product-images')}`}>
                                    <i className="nav-icon fas fa-images"></i>
                                    <p>Hình ảnh SP</p>
                                </Link>
                            </li>

                            <li className="nav-header">KINH DOANH</li>

                            <li className="nav-item">
                                <Link to="/orders" className={`nav-link ${isActive('/orders')}`}>
                                    <i className="nav-icon fas fa-shopping-cart"></i>
                                    <p>Đơn hàng</p>
                                </Link>
                            </li>
                            <li className="nav-item">
                                <Link to="/discounts" className={`nav-link ${isActive('/discounts')}`}>
                                    <i className="nav-icon fas fa-percentage"></i>
                                    <p>Mã giảm giá</p>
                                </Link>
                            </li>

                            {isAdmin() && (
                                <>
                                    <li className="nav-header">QUẢN TRỊ</li>
                                    <li className="nav-item">
                                        <Link to="/users" className={`nav-link ${isActive('/users')}`}>
                                            <i className="nav-icon fas fa-users"></i>
                                            <p>Người dùng</p>
                                        </Link>
                                    </li>
                                </>
                            )}
                        </ul>
                    </nav>
                </div>
            </aside>

            {children}

            <footer className="main-footer">
                <strong>Copyright &copy; 2024 <a href="#">Trang Sức Bạc</a>.</strong>
                <div className="float-right d-none d-sm-inline-block">
                    <b>Version</b> 1.0.0
                </div>
            </footer>
        </div>
    );
};

export default MainLayout;
