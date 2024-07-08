import {
  createFileRoute,
  Link,
  redirect,
  useRouteContext,
} from "@tanstack/react-router";
import { allUserOptions } from "../../queries/user-queries";
import { useSuspenseQuery } from "@tanstack/react-query";

const Users = () => {
  const { data } = useSuspenseQuery(allUserOptions);
  const { userId } = useRouteContext({ from: "/users/" });

  const sorted = [...data].sort((a, b) => a.name.localeCompare(b.name));

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
                  <Link to={`/users/${r.id}/edit`}>Edit</Link>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
};

export const Route = createFileRoute("/users/")({
  component: Users,
  loader: ({ context }) => {
    return context.queryClient.ensureQueryData(allUserOptions);
  },
  beforeLoad: ({ context }) => {
    if (!context.loggedIn) {
      throw redirect({
        to: "/",
      });
    }
    if (!context.canViewUsers) {
      throw redirect({
        to: "/home",
      });
    }
  },
});
