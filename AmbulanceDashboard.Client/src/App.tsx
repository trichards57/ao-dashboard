import { RouterProvider, createBrowserRouter } from "react-router-dom";
import Home from "./home";
import type { PublicClientApplication } from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";
import Logout from "./logout";
import VehicleStatus from "./status";
import VehicleConfiguration from "./configuration";

const router = createBrowserRouter([
  {
    path: "/",
    element: <Home />,
  },
  {
    path: "/front-channel-logout",
    element: <Logout />,
  },
  {
    path: "/vehicles",
    children: [
      {
        path: "status",
        element: <VehicleStatus />,
      },
      {
        path: "config",
        element: <VehicleConfiguration />,
      },
    ],
  },
]);

if (import.meta.hot) {
  import.meta.hot.dispose(() => router.dispose());
}

function App({ pca }: { pca: PublicClientApplication }) {
  return (
    <MsalProvider instance={pca}>
      <RouterProvider router={router} fallbackElement={<p>Loading...</p>} />
    </MsalProvider>
  );
}

export default App;
