import {
  createFileRoute,
  redirect,
  useNavigate,
  useParams,
  useRouteContext,
} from "@tanstack/react-router";
import { userSettings, useUpdateUser } from "../../queries/user-queries";
import { useState } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { allRoleOptions } from "../../queries/role-queries";
import { useTitle } from "../../components/useTitle";

const EditUser = () => {
  const { isAdmin } = useRouteContext({ from: "/users/edit/$id" });
  const { id } = useParams({ from: "/users/edit/$id" });
  const { data } = useSuspenseQuery(userSettings(id));
  const { data: roles } = useSuspenseQuery(allRoleOptions);
  const { mutateAsync } = useUpdateUser(id);
  const navigate = useNavigate();

  const [role, setRole] = useState(data.roleId);

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
              {role == "None" && <option value="None">None</option>}
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
};

export const Route = createFileRoute("/users/edit/$id")({
  component: EditUser,
  loader: ({ params, context }) => {
    return Promise.all([
      context.queryClient.ensureQueryData(userSettings(params.id)),
      context.queryClient.ensureQueryData(allRoleOptions),
    ]);
  },
  beforeLoad: ({ context, params }) => {
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
    if (params.id.toUpperCase() === context.userId.toUpperCase()) {
      throw redirect({
        to: "/users",
      });
    }
  },
});
