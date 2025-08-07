import { Link } from 'react-router-dom';

function Register() {
  return (
    <div className="auth-container">
      <form className="auth-form">
        <h2>Register</h2>
        <input type="text" placeholder="Full Name" required />
        <input type="email" placeholder="Email" required />
        <input type="password" placeholder="Password" required />
        <button type="submit">Register</button>

        {/* Add this below the button */}
        <p style={{ textAlign: 'center', marginTop: '1rem' }}>
          Already have an account?{' '}
          <Link to="/login" style={{ color: '#007bff', textDecoration: 'none' }}>
            Login here
          </Link>
        </p>
      </form>
    </div>
  );
}

export default Register;
