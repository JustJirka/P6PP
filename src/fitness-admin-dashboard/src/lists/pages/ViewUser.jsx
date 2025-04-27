import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button, Spinner, Alert, Badge, Row } from 'react-bootstrap';
import Api from '../../utils/Api';
import UserDetailCard from './components/UserDetailCard';
import UserInfoRow from './components/UserInfoRow';

const ViewUser = () => {
  const { userId } = useParams();
  const navigate = useNavigate();

  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        setLoading(true);
        const response = await Api.get(`/api/user/${userId}`);
        const userData = response?.data?.user || response?.data || response;

        if (!userData || !userData.id) {
          throw new Error('Invalid user data received from API');
        }

        setUser({
          id: userData.id,
          username: userData.username || 'N/A',
          email: userData.email || 'N/A',
          firstName: userData.firstName || '',
          lastName: userData.lastName || '',
          phoneNumber: userData.phoneNumber || 'N/A',
          state: userData.state || 'N/A',
          roleId: userData.roleId,
          role: userData.role?.name || 'N/A',
          roleDescription: userData.role?.description || '',
          sex: userData.sex || 'N/A',
          weight: userData.weight ?? 'N/A',
          height: userData.height ?? 'N/A',
          dateOfBirth: userData.dateOfBirth
            ? new Date(userData.dateOfBirth).toLocaleDateString()
            : 'N/A',
          createdOn: userData.createdOn
            ? new Date(userData.createdOn).toLocaleDateString()
            : 'N/A',
          updatedOn: userData.updatedOn
            ? new Date(userData.updatedOn).toLocaleDateString()
            : 'N/A',
        });

        setError(null);
      } catch (err) {
        console.error('Error while loading user:', err);
        setError(`Failed to load user: ${err.message}`);
        setUser(null);
      } finally {
        setLoading(false);
      }
    };

    if (userId) {
      fetchUser();
    }
  }, [userId]);

  const handleEdit = () => navigate(`/admin/users/edit/${userId}`);
  const handleBack = () => navigate('/admin/users');

  if (loading) {
    return (
      <div className="text-center my-5">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </div>
    );
  }

  if (error) {
    return (
      <UserDetailCard>
        <Alert variant="danger">{error}</Alert>
        <div className="text-center">
          <Button variant="primary" onClick={handleBack}>
            Back to user list
          </Button>
        </div>
      </UserDetailCard>
    );
  }

  if (!user) {
    return (
      <UserDetailCard>
        <div className="text-center">
          <h5>User not found</h5>
          <p>The requested user does not exist or has been deleted.</p>
          <Button variant="primary" onClick={handleBack}>
            Back to user list
          </Button>
        </div>
      </UserDetailCard>
    );
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>User Details</h2>
        <div>
          <Button variant="primary" className="me-2" onClick={handleEdit}>
            Edit
          </Button>
          <Button variant="secondary" onClick={handleBack}>
            Back
          </Button>
        </div>
      </div>

      <UserDetailCard>
        <Row className="mb-3">
          <UserInfoRow label="ID" value={user.id} />
          <UserInfoRow label="Username" value={user.username} />
          <UserInfoRow label="State" value={
            <Badge bg={user.state === 'Active' ? 'success' : 'secondary'}>
              {user.state}
            </Badge>
          } />
          <UserInfoRow label="Role" value={
            <>
              <Badge bg="info">{user.role}</Badge>
              {user.roleDescription && (
                <div className="small text-muted mt-1">{user.roleDescription}</div>
              )}
            </>
          } colSize={{ xs: 12, md: 6 }} />
        </Row>
      </UserDetailCard>

      <UserDetailCard title="Contact Information">
        <Row>
          <UserInfoRow label="Email" value={user.email} />
          <UserInfoRow label="Phone" value={user.phoneNumber} />
        </Row>
      </UserDetailCard>

      <UserDetailCard title="Personal Information">
        <Row>
          <UserInfoRow label="First Name" value={user.firstName} />
          <UserInfoRow label="Last Name" value={user.lastName} />
          <UserInfoRow label="Gender" value={user.sex} />
          <UserInfoRow label="Height" value={user.height === 'N/A' ? 'N/A' : `${user.height} cm`} />
          <UserInfoRow label="Weight" value={user.weight === 'N/A' ? 'N/A' : `${user.weight} kg`} />
        </Row>
      </UserDetailCard>

      <UserDetailCard title="System Information">
        <Row>
          <UserInfoRow label="Role ID" value={user.roleId} />
          <UserInfoRow label="Created On" value={user.createdOn} />
          <UserInfoRow label="Last Updated" value={user.updatedOn} />
        </Row>
      </UserDetailCard>
    </div>
  );
};

export default ViewUser;
