import { createFileRoute, Link } from "@tanstack/react-router";
import { preloadRoles, useRoles } from "../../queries/role-queries";
import { useTitle } from "../../components/useTitle";
import {
  redirectIfLoggedOut,
  redirectIfNoPermission,
} from "../../support/check-logged-in";

export function Roles() {
  const { data } = useRoles();
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
                  <Link to="/roles/edit/$id" params={{ id: r.id }}>
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

export const Route = createFileRoute("/roles/")({
  component: Roles,
  loader: ({ context }) => preloadRoles(context.queryClient),
  beforeLoad: ({ context }) => {
    redirectIfLoggedOut(context);
    redirectIfNoPermission(context.canEditRoles);
  },
});
