import { createFileRoute, Link, redirect } from "@tanstack/react-router";
import { allRoleOptions } from "../../queries/role-queries";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useTitle } from "../../components/useTitle";

const Roles = () => {
  const { data } = useSuspenseQuery(allRoleOptions);
  useTitle("User Roles");

  const roles = [...data].sort((a, b) => a.name.localeCompare(b.name));

  return (
    <>
      <h1 className="title">User Roles</h1>

      <table className="table is-striped is-fullwidth">
        <thead>
          <tr>
            <th>Role</th>
            <th>Vehicle Configuration</th>
            <th>VOR Data</th>
            <th>User Role</th>
            <th>Role Permissions</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {roles.map((r) => (
            <tr key={r.id}>
              <td>{r.name}</td>
              <td>{r.vehicleConfiguration}</td>
              <td>{r.vorData}</td>
              <td>{r.permissions}</td>
              <td>{r.name == "Administrator" ? "Write" : "Deny"}</td>
              <td className="edit">
                {r.name != "Administrator" && (
                  <Link to={`/roles/edit/${r.id}`}>Edit</Link>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
};

export const Route = createFileRoute("/roles/")({
  component: Roles,
  loader: ({ context }) => {
    return context.queryClient.ensureQueryData(allRoleOptions);
  },
  beforeLoad: ({ context }) => {
    if (!context.loggedIn) {
      throw redirect({
        to: "/",
      });
    }
    if (!context.canEditRoles) {
      throw redirect({
        to: "/home",
      });
    }
  },
});
