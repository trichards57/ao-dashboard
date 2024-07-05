import { StrictMode } from "react";
import ReactDOM from "react-dom/client";
import { createRouter } from "@tanstack/react-router";

// Import the generated route tree
import { routeTree } from "./routeTree.gen";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

import "./index.css";
import { InnerApp } from "./App";

const queryClient = new QueryClient();

// Create a new router instance
const router = createRouter({
  routeTree,
  context: {
    queryClient,
    realName: "",
    userId: "",
    email: "",
    role: "",
    amrUsed: "",
    lastAuthenticated: "",
    otherClaims: {} as Record<string, string>,
  },
});

// Register the router instance for type safety
declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

// Render the app
const rootElement = document.getElementById("root")!;
if (!rootElement.innerHTML) {
  const root = ReactDOM.createRoot(rootElement);
  root.render(
    <StrictMode>
      <QueryClientProvider client={queryClient}>
        <InnerApp router={router} queryClient={queryClient} />
      </QueryClientProvider>
    </StrictMode>,
  );
}
