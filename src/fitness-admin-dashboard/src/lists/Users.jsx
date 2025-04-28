import React, { useState, useEffect } from 'react';
import { Table, Dropdown, ButtonGroup, Button, Spinner, Alert } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Api from '../utils/Api';

const Users = () => {
  const navigate = useNavigate();
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        setLoading(true);
        const result = await Api.get('/api/users');
        const userList = result?.data?.users || [];

        const mappedUsers = userList.map(user => ({
          id: user.id,
          name: `${user.firstName} ${user.lastName}`,
          email: user.email,
          role: user.role?.name || 'N/A',
          date: new Date(user.createdOn).toLocaleDateString()
        }));

        setUsers(mappedUsers);
        setError(null);
      } catch (err) {
        if (err.message === 'Failed to fetch') {
          setError('Cannot contact the API server.');
        } else {
          setError(`Failed to load users: ${err.message}`);
        }
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, []);

  const handleEdit = (userId) => {
    navigate(`/admin/users/edit/${userId}`);
  };

  const handleDelete = async (userId) => {
    if (!window.confirm('Are you sure you want to delete this user?')) return;

    try {
      await Api.delete(`/api/user/${userId}`);
      setUsers(prev => prev.filter(user => user.id !== userId));
    } catch (error) {
      alert(`Failed to delete user: ${error.message}`);
    }
  };

  const handleCreate = () => {
    navigate('/admin/users/create');
  };

  const handleRowClick = (userId, e) => {
    if (e.target.closest('.dropdown')) return;
    navigate(`/admin/users/view/${userId}`);
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Users</h2>
        <Button variant="primary" onClick={handleCreate}>
          + New User
        </Button>
      </div>

      {error && <Alert variant="danger">{error}</Alert>}

      {loading ? (
        <div className="text-center my-5">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Loading...</span>
          </Spinner>
        </div>
      ) : (
        <Table hover responsive bordered className="align-middle">
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Email</th>
              <th>Role</th>
              <th>Registered</th>
              <th style={{ width: '60px' }}></th>
            </tr>
          </thead>
          <tbody>
            {users.length === 0 ? (
              <tr>
                <td colSpan="6" className="text-center">No users found.</td>
              </tr>
            ) : (
              users.map(user => (
                <tr
                  key={user.id}
                  onClick={(e) => handleRowClick(user.id, e)}
                  style={{ cursor: 'pointer' }}
                >
                  <td>{user.id}</td>
                  <td>{user.name}</td>
                  <td><span className="badge bg-light text-dark">{user.email}</span></td>
                  <td>{user.role}</td>
                  <td>{user.date}</td>
                  <td>
                    <Dropdown as={ButtonGroup} className="dropdown">
                      <Dropdown.Toggle variant="light" size="sm" className="border-0">
                        ⋯
                      </Dropdown.Toggle>
                      <Dropdown.Menu align="end">
                        <Dropdown.Item onClick={() => handleEdit(user.id)}>Edit</Dropdown.Item>
                        <Dropdown.Item onClick={() => handleDelete(user.id)} className="text-danger">Delete</Dropdown.Item>
                      </Dropdown.Menu>
                    </Dropdown>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </Table>
      )}
    </div>
  );
};

export default Users;
