import { useEffect } from "react";
import useUser from "../hooks/useUser";
import "./landing.css";
import { useNavigate } from "react-router-dom";

function App() {
  const { isLoading, data } = useUser();
  const navigate = useNavigate();

  useEffect(() => {
    if (!isLoading) {
      if (data) {
        navigate("/home");
      }
    }
  }, [navigate, isLoading, data]);

  const login = () => {
    location.assign("/.auth/login/aad");
  };

  return (
    <div className="card">
      <h1>AO Dashboard</h1>
      <button type="button" disabled={isLoading} onClick={login}>
        Sign in with Azure AD
      </button>
    </div>
  );
}

export default App;
