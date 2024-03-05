import { useNavigate } from "@tanstack/react-router";

import { useIsAuthenticated } from "../api-hooks/users";

// eslint-disable-next-line import/prefer-default-export
export function useRequireAuth() {
  const { data: isAuthenticated, isLoading: authLoading } =
    useIsAuthenticated();
  const navigate = useNavigate();

  if (!authLoading && !isAuthenticated) {
    navigate({ to: "/" });
  }
}
