import { useMsal } from "@azure/msal-react";

const Logout = () => {
  const { instance } = useMsal();

  localStorage.clear();
  instance.logoutRedirect({
    onRedirectNavigate: () => {
      // Return false to stop navigation after local logout
      return false;
    },
  });

  return (
    <div>
      <p>Logging out...</p>
    </div>
  );
};

export default Logout;
