import { Link, createFileRoute } from "@tanstack/react-router";

import useTitle from "../../components/useTitle";
import { preloadUsers, useUsers } from "../../queries/user-queries";
import {
  redirectIfLoggedOut,
  redirectIfNoPermission,
} from "../../support/check-logged-in";

export function Users({
  userId,
  isAdmin = false,
}: {
  userId: string;
  isAdmin?: boolean;
}) {
  const { data } = useUsers();

  useTitle("User Settings");

  const sorted = data
    .toSorted((a, b) => a.name.localeCompare(b.name))
    .filter((r) => r.role !== "Administrator" || isAdmin);

  return (
    <>
      <h1 className="title">User Settings</h1>

      <table className="table is-striped is-fullwidth">
        <thead>
          <tr>
            <th>User</th>
            <th>Role</th>
            <th aria-label="Actions" />
          </tr>
        </thead>
        <tbody>
          {sorted.map((r) => (
            <tr key={r.id}>
              <td>{r.name}</td>
              <td>{r.role ?? "None"}</td>
              <td className="edit">
                {r.id.toUpperCase() !== userId.toUpperCase() && (
                  <Link to="/users/edit/$id" params={{ id: r.id }}>
                    Edit
                  </Link>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}

export const Route = createFileRoute("/users/")({
  component: function Component() {
    return (
      <Users
        userId={Route.useRouteContext().userId}
        isAdmin={Route.useRouteContext().isAdmin}
      />
    );
  },
  loader: ({ context }) => preloadUsers(context.queryClient),
  beforeLoad: ({ context }) => {
    redirectIfLoggedOut(context);
    redirectIfNoPermission(context.canViewUsers);
  },
});
