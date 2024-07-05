import { QueryClient, useSuspenseQuery } from "@tanstack/react-query";
import { userMeOptions } from "./queries/user-queries";
import { RouterProvider } from "@tanstack/react-router";

export const InnerApp = ({
  queryClient,
  router,
}: {
  queryClient: QueryClient;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  router: any;
}) => {
  const meQuery = useSuspenseQuery(userMeOptions);

  const context = {
    queryClient,
    ...meQuery.data,
    loggedIn: meQuery.data !== null,
  };

  return <RouterProvider router={router} context={context} />;
};
