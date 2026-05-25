import React, { useState, useEffect } from 'react';
import { productApi, categoryApi, materialApi, orderApi, userApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const Dashboard = () => {
    const [stats, setStats] = useState({
        products: 0,
        categories: 0,
        materials: 0,
        orders: 0,
        users: 0,
    });
    const [loading, setLoading] = useState(true);
    const { isAdmin, user } = useAuth();

    useEffect(() => {
        loadStats();
    }, [user]);

    const loadStats = async () => {
        setLoading(true);
        try {
            const [productsRes, categoriesRes, materialsRes] = await Promise.all([
                productApi.getAll({ page: 1, pageSize: 1 }),
                categoryApi.getAll({ page: 1, pageSize: 1 }),
                materialApi.getAll({ page: 1, pageSize: 1 }),
            ]);

            const newStats = {
                products: productsRes.data?.totalCount || 0,
                categories: categoriesRes.data?.totalCount || 0,
                materials: materialsRes.data?.totalCount || 0,
                orders: 0,
                users: 0,
            };

            if (isAdmin()) {
                try {
                    const [ordersRes, usersRes] = await Promise.all([
                        orderApi.getAll({ page: 1, pageSize: 1 }),
                        userApi.getAll({ page: 1, pageSize: 1 }),
                    ]);
                    newStats.orders = ordersRes.data?.totalCount || 0;
                    newStats.users = usersRes.data?.totalCount || 0;
                } catch {
                    // Non-admin or permission error — keep 0
                }
            }

            setStats(newStats);
        } catch (error) {
            console.error('Failed to load stats:', error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="content-wrapper">
            <div className="content-header">
                <div className="container-fluid">
                    <div className="row mb-2">
                        <div className="col-sm-6">
                            <h1 className="m-0">Dashboard</h1>
                        </div>
                    </div>
                </div>
            </div>

            <section className="content">
                <div className="container-fluid">
                    {loading ? (
                        <div className="text-center py-5">
                            <div className="spinner-border text-primary" role="status">
                                <span className="sr-only">Loading...</span>
                            </div>
                        </div>
                    ) : (
                        <div className="row">
                            <div className="col-lg-4 col-6">
                                <div className="small-box bg-info">
                                    <div className="inner">
                                        <h3>{stats.products}</h3>
                                        <p>Sản phẩm</p>
                                    </div>
                                    <div className="icon">
                                        <i className="fas fa-box"></i>
                                    </div>
                                    <a href="/products" className="small-box-footer">
                                        Xem chi tiết <i className="fas fa-arrow-circle-right"></i>
                                    </a>
                                </div>
                            </div>
                            <div className="col-lg-4 col-6">
                                <div className="small-box bg-success">
                                    <div className="inner">
                                        <h3>{stats.categories}</h3>
                                        <p>Danh mục</p>
                                    </div>
                                    <div className="icon">
                                        <i className="fas fa-tags"></i>
                                    </div>
                                    <a href="/categories" className="small-box-footer">
                                        Xem chi tiết <i className="fas fa-arrow-circle-right"></i>
                                    </a>
                                </div>
                            </div>
                            <div className="col-lg-4 col-6">
                                <div className="small-box bg-purple" style={{ backgroundColor: '#6f42c1' }}>
                                    <div className="inner">
                                        <h3>{stats.materials}</h3>
                                        <p>Chất liệu</p>
                                    </div>
                                    <div className="icon">
                                        <i className="fas fa-gem"></i>
                                    </div>
                                    <a href="/materials" className="small-box-footer">
                                        Xem chi tiết <i className="fas fa-arrow-circle-right"></i>
                                    </a>
                                </div>
                            </div>
                            {isAdmin() && (
                                <>
                                    <div className="col-lg-4 col-6">
                                        <div className="small-box bg-warning">
                                            <div className="inner">
                                                <h3>{stats.orders}</h3>
                                                <p>Đơn hàng</p>
                                            </div>
                                            <div className="icon">
                                                <i className="fas fa-shopping-cart"></i>
                                            </div>
                                            <a href="/orders" className="small-box-footer">
                                                Xem chi tiết <i className="fas fa-arrow-circle-right"></i>
                                            </a>
                                        </div>
                                    </div>
                                    <div className="col-lg-4 col-6">
                                        <div className="small-box bg-danger">
                                            <div className="inner">
                                                <h3>{stats.users}</h3>
                                                <p>Người dùng</p>
                                            </div>
                                            <div className="icon">
                                                <i className="fas fa-users"></i>
                                            </div>
                                            <a href="/users" className="small-box-footer">
                                                Xem chi tiết <i className="fas fa-arrow-circle-right"></i>
                                            </a>
                                        </div>
                                    </div>
                                </>
                            )}
                        </div>
                    )}

                    <div className="row">
                        <div className="col-12">
                            <div className="card">
                                <div className="card-header">
                                    <h3 className="card-title">Chào mừng đến với hệ thống quản trị</h3>
                                </div>
                                <div className="card-body">
                                    <p>Đây là hệ thống quản lý cửa hàng trang sức bạc:</p>
                                    <ul>
                                        <li><strong>Backend:</strong> .NET Core 8.0 với Entity Framework Core</li>
                                        <li><strong>Frontend:</strong> React 18 với React Router</li>
                                        <li><strong>UI:</strong> AdminLTE 3 với Bootstrap 4</li>
                                        <li><strong>Authentication:</strong> JWT Bearer Token</li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    );
};

export default Dashboard;
