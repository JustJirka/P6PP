import React, { useState, useEffect } from 'react';
import { Container, Alert, Spinner } from 'react-bootstrap';
import { useNavigate, useParams } from 'react-router-dom';
import UserForm from '../forms/UserForm';
import Api from '../../utils/Api';

const EditUser = () => {
  const { userId } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [userData, setUserData] = useState(null);
  const [roles, setRoles] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);

        const userResponse = await Api.get(`/api/user/${userId}`);
        const user = userResponse.data?.user || userResponse.data || userResponse;

        const processedUser = {
          id: user.id,
          username: user.username || '',
          firstName: user.firstName || '',
          lastName: user.lastName || '',
          email: user.email || '',
          phoneNumber: user.phoneNumber || '',
          weight: user.weight ?? '',
          height: user.height ?? '',
          sex: user.sex || '',
          state: user.state === 'Active' ? 'Active' : 'Inactive',
          roleId: user.role?.id || null,
        };

        setUserData(processedUser);

        const roleResponse = await Api.getRoles();
        setRoles(roleResponse.data || []);

        setError(null);
      } catch (err) {
        setError(`Error loading data: ${err.message}`);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [userId]);

  const handleSubmit = async (formData) => {
    try {
      setSubmitting(true);
      setError(null);

      const updateData = {
        username: formData.username,
        firstName: formData.firstName,
        lastName: formData.lastName,
        email: formData.email,
        phoneNumber: formData.phoneNumber || '',
        sex: formData.sex || '',
        weight: formData.weight ? Number(formData.weight) : null,
        height: formData.height ? Number(formData.height) : null,
      };

      // Update user details
      await Api.put(`/api/user/${userId}`, updateData);

      // If role changed, update the role separately
      if (formData.roleId !== userData.roleId) {
        await Api.changeUserRole(userData.id, formData.roleId);
      }

      navigate('/admin/users');
    } catch (err) {
      setError(`Error saving user: ${err.message}`);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <Container>
        <div className="text-center my-5">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Loading...</span>
          </Spinner>
        </div>
      </Container>
    );
  }

  return (
    <Container>
      <h2 className="mb-4">Edit User</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      <UserForm
        onSubmit={handleSubmit}
        initialData={userData}
        isEditing={true}
        roles={roles}
        submitting={submitting}
      />
    </Container>
  );
};

export default EditUser;
