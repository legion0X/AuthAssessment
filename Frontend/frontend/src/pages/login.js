import LoginForm from "../components/LoginForm";
import "../styles/login.css";

export default function LoginPage() {
  return (
    <div className="login-container">
      <div className="login-box">
        <div className="login-icon">
          <div className="login-icon-circle">
            <svg viewBox="0 0 20 20" fill="currentColor">
              <path fillRule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clipRule="evenodd" />
            </svg>
            <span className="plus">+</span>
          </div>
        </div>

        <h1 className="login-title">Login</h1>
        <p className="login-subtitle">Enter your details to login.</p>

        <LoginForm />

        <p className="login-footer">
          Already have an account? <a href="#" className="login-link">Login</a>
        </p>
      </div>
    </div>
  );
}