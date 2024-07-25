import { createFileRoute, useNavigate } from "@tanstack/react-router";
import {
  Permission,
  preloadRole,
  useRole,
  useUpdateRole,
} from "../../queries/role-queries";
import { useState } from "react";
import { useTitle } from "../../components/useTitle";
import {
  redirectIfLoggedOut,
  redirectIfNoPermission,
} from "../../support/check-logged-in";

function EditRole({ id }: { id: string }) {
  const { data } = useRole(id);
  const { mutateAsync } = useUpdateRole(id);
  const navigate = useNavigate();

  const [vehicleConfiguration, setVehicleConfiguration] = useState(
    data.vehicleConfiguration,
  );
  const [vorData, setVorData] = useState(data.vorData);
  const [permissions, setPermissions] = useState(data.permissions);

  const [running, setRunning] = useState(false);

  useTitle(`Update ${data.name}`);

  async function updateRole() {
    setRunning(true);
    await mutateAsync({
      vorData,
      permissions,
      vehicleConfiguration,
    });
    setRunning(false);
    navigate({ to: "/roles" });
  }

  return (
    <>
      <h1 className="title">Edit {data.name}</h1>
      <form
        onSubmit={(e) => {
          e.stopPropagation();
          e.preventDefault();
          updateRole();
        }}
        noValidate
      >
        <div className="field">
          <label className="label" htmlFor="vehicle-configuration">
            Vehicle Configuration
          </label>
          <div className="select is-fullwidth">
            <select
              disabled={running}
              id="vehicle-configuration"
              value={vehicleConfiguration}
              onChange={(e) =>
                setVehicleConfiguration(e.target.value as Permission)
              }
              required
            >
              <option value="Deny">Deny</option>
              <option value="Read">Read</option>
              <option value="Write">Read/Write</option>
            </select>
          </div>
        </div>
        <div className="field">
          <label className="label" htmlFor="vor-data">
            VOR Data
          </label>
          <div className="select is-fullwidth">
            <select
              disabled={running}
              id="vor-data"
              value={vorData}
              onChange={(e) => setVorData(e.target.value as Permission)}
              required
            >
              <option value="Deny">Deny</option>
              <option value="Read">Read</option>
              <option value="Write">Read/Write</option>
            </select>
          </div>
        </div>
        <div className="field">
          <label className="label" htmlFor="user-role">
            Permissions
          </label>
          <div className="select is-fullwidth">
            <select
              disabled={running}
              id="user-role"
              value={permissions}
              onChange={(e) => setPermissions(e.target.value as Permission)}
              required
            >
              <option value="Deny">Deny</option>
              <option value="Read">Read</option>
              <option value="Write">Read/Write</option>
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

export const Route = createFileRoute("/roles/edit/$id")({
  component: function Component() {
    return <EditRole id={Route.useParams().id} />;
  },
  loader: ({ params, context }) => preloadRole(context.queryClient, params.id),
  beforeLoad: ({ context }) => {
    redirectIfLoggedOut(context);
    redirectIfNoPermission(context.canEditRoles);
  },
});
