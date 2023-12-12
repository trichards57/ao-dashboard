import React from "react";
import ReactDOM from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import Landing from "./landing";
import "./index.css";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { ErrorBoundary } from "react-error-boundary";
import Error from "./error";
import Home from "./home";
import Config from "./vehicles/config";

const router = createBrowserRouter([
  { path: "/", element: <Landing /> },
  { path: "/home", element: <Home /> },
  { path: "/vehicles/config", element: <Config /> },
  { path: "*", element: <Error /> },
]);
const client = new QueryClient();

ReactDOM.createRoot(document.getElementById("root")!).render(
  <ErrorBoundary fallback={<Error />}>
    <QueryClientProvider client={client}>
      <React.StrictMode>
        <RouterProvider router={router} />
        <ReactQueryDevtools initialIsOpen={false} />
      </React.StrictMode>
    </QueryClientProvider>
  </ErrorBoundary>
);
