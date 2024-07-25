import { createFileRoute, Link, redirect } from "@tanstack/react-router";
import { preloadUsers, useUsers } from "../../queries/user-queries";
import { useTitle } from "../../components/useTitle";

function Users({ userId, isAdmin }: { userId: string; isAdmin: boolean }) {
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
            <th></th>
          </tr>
        </thead>
        <tbody>
          {sorted.map((r) => (
            <tr key={r.id}>
              <td>{r.name}</td>
              <td>{r.role ?? "None"}</td>
              <td className="edit">
                {r.id.toUpperCase() !== userId.toUpperCase() && (
                  <Link to={`/users/edit/${r.id}`}>Edit</Link>
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
    if (!context.loggedIn) {
      throw redirect({
        to: "/",
      });
    }
    if (!context.canViewUsers) {
      throw redirect({
        to: "/home",
        search: { region: "All", district: "All", hub: "All" },
      });
    }
  },
});
