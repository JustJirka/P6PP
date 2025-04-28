import { Card } from 'react-bootstrap';

const FormSection = ({ title, children }) => (
  <Card className="mb-4">
    <Card.Header as="h5">{title}</Card.Header>
    <Card.Body>
      {children}
    </Card.Body>
  </Card>
);

export default FormSection;
