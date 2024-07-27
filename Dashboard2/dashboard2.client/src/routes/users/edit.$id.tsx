import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import { useState } from "react";

import useTitle from "../../components/useTitle";
import { preloadRoles, useRoles } from "../../queries/role-queries";
import {
  preloadUser,
  useUpdateUser,
  useUser,
} from "../../queries/user-queries";
import {
  redirectIfLoggedOut,
  redirectIfNoPermission,
} from "../../support/check-logged-in";

export function EditUser({
  id,
  isAdmin = false,
}: {
  id: string;
  isAdmin?: boolean;
}) {
  const { data } = useUser(id);
  const { data: roles } = useRoles();
  const { mutateAsync } = useUpdateUser(id);
  const navigate = useNavigate();

  const [role, setRole] = useState(data.roleId ?? "None");

  const [running, setRunning] = useState(false);

  useTitle(`Update ${data.name}`);

  async function updateUser() {
    setRunning(true);
    await mutateAsync({
      roleId: role,
    });
    setRunning(false);
    navigate({ to: "/users" });
  }

  return (
    <>
      <h1 className="title">Update {data.name}</h1>
      <form
        onSubmit={(e) => {
          e.stopPropagation();
          e.preventDefault();
          updateUser();
        }}
        noValidate
      >
        <div className="field">
          <label className="label" htmlFor="role">
            Role
          </label>
          <div className="select is-fullwidth">
            <select
              disabled={running}
              id="role"
              value={role}
              onChange={(e) => setRole(e.target.value)}
              required
            >
              {role === "None" && <option value="None">None</option>}
              {roles
                .filter((r) => r.name !== "Administrator" || isAdmin)
                .map((r) => (
                  <option key={r.id} value={r.id}>
                    {r.name}
                  </option>
                ))}
            </select>
          </div>
        </div>
        <button
          disabled={running}
          className="button is-primary is-fullwidth"
          type="submit"
        >
          Save
        </button>
      </form>
    </>
  );
}

export const Route = createFileRoute("/users/edit/$id")({
  component: function Component() {
    return (
      <EditUser
        id={Route.useParams().id}
        isAdmin={Route.useRouteContext().isAdmin}
      />
    );
  },
  loader: ({ params, context }) => {
    return Promise.all([
      preloadUser(context.queryClient, params.id),
      preloadRoles(context.queryClient),
    ]);
  },
  beforeLoad: ({ context, params }) => {
    redirectIfLoggedOut(context);
    redirectIfNoPermission(context.canEditUsers);
    if (params.id.toUpperCase() === context.userId.toUpperCase()) {
      // eslint-disable-next-line @typescript-eslint/no-throw-literal
      throw redirect({
        to: "/users",
      });
    }
  },
});
