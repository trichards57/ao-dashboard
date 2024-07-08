import {
  createFileRoute,
  useNavigate,
  useParams,
} from "@tanstack/react-router";
import {
  useUpdateVehicle,
  vehicleSettings,
} from "../../queries/vehicle-queries";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useState } from "react";

const EditVehicle = () => {
  const { id } = useParams({ from: "/vehicles/edit/$id" });
  const { data } = useSuspenseQuery(vehicleSettings(id));
  const { mutateAsync } = useUpdateVehicle();
  const navigate = useNavigate();

  const [callSign, setCallSign] = useState(data.callSign);
  const [vehicleType, setVehicleType] = useState(data.vehicleType);
  const [region, setRegion] = useState(data.region);
  const [district, setDistrict] = useState(data.district);
  const [hub, setHub] = useState(data.hub);
  const [forDisposal, setForDisposal] = useState(data.forDisposal);

  const [callSignDirty, setCallSignDirty] = useState(false);
  const [districtDirty, setDistrictDirty] = useState(false);
  const [hubDirty, setHubDirty] = useState(false);

  const [running, setRunning] = useState(false);

  const callSignValid = callSign.length > 0;
  const districtValid = district.length > 0;
  const hubValid = hub.length > 0;

  function getClass(dirty: boolean, valid: boolean) {
    if (!dirty) return "";
    if (valid) return "is-success";
    return "is-danger";
  }

  const callSignClass = getClass(callSignDirty, callSignValid);
  const districtClass = getClass(districtDirty, districtValid);
  const hubClass = getClass(hubDirty, hubValid);

  async function updateVehicle() {
    if (!callSignValid || !districtValid || !hubValid) return;

    setRunning(true);
    await mutateAsync({
      registration: data.registration,
      callSign,
      vehicleType,
      region,
      district,
      hub,
      forDisposal,
    });
    setRunning(false);
    navigate({ to: "/vehicles/config" });
  }

  return (
    <>
      <h1 className="title">Edit {data.registration}</h1>

      <form
        onSubmit={(e) => {
          e.stopPropagation();
          e.preventDefault();
          updateVehicle();
        }}
        noValidate
      >
        <div className="field">
          <label className="label" htmlFor="call-sign">
            Call Sign
          </label>
          <div className="control">
            <input
              disabled={running}
              type="text"
              className={`input ${callSignClass}`}
              id="call-sign"
              value={callSign}
              onChange={(e) => {
                setCallSign(e.target.value);
                setCallSignDirty(true);
              }}
              aria-invalid={!callSignValid}
            />
            {!callSignValid && callSignDirty && (
              <p className="help is-danger">Call Sign is required</p>
            )}
          </div>
        </div>
        <div className="field">
          <label className="label" htmlFor="registration">
            Registration
          </label>
          <div className="control">
            <input
              type="text"
              className="input"
              id="registration"
              value={data.registration}
              disabled
            />
          </div>
        </div>
        <div className="field">
          <label className="label" htmlFor="vehicle-type">
            Vehicle Type
          </label>
          <div className="select is-fullwidth">
            <select
              disabled={running}
              id="vehicle-type"
              value={vehicleType}
              onChange={(e) => setVehicleType(e.target.value)}
            >
              <option value="Other">Other</option>
              <option value="FrontLineAmbulance">Front-Line Ambulance</option>
              <option value="AllWheelDrive">All Wheel Drive</option>
              <option value="OffRoadAmbulance">Off-Road Ambulance</option>
            </select>
          </div>
        </div>
        <div className="field">
          <label className="label" htmlFor="region">
            Region
          </label>
          <div className="select is-fullwidth">
            <select
              disabled={running}
              id="region"
              value={region}
              onChange={(e) => setRegion(e.target.value)}
            >
              <option value="Unknown">Unknown</option>
              <option value="EastOfEngland">East of England</option>
              <option value="EastMidlands">East Midlands</option>
              <option value="London">London</option>
              <option value="NorthEast">North East</option>
              <option value="NorthWest">North West</option>
              <option value="SouthEast">South East</option>
              <option value="SouthWest">South West</option>
              <option value="WestMidlands">West Midlands</option>
            </select>
          </div>
        </div>
        <div className="field">
          <label className="label" htmlFor="district">
            District
          </label>
          <div className="control">
            <input
              disabled={running}
              type="text"
              className={`input ${districtClass}`}
              id="district"
              value={district}
              onChange={(e) => {
                setDistrict(e.target.value);
                setDistrictDirty(true);
              }}
              aria-invalid={!districtValid}
            />
            {!districtValid && districtDirty && (
              <p className="help is-danger">District is required</p>
            )}
          </div>
        </div>
        <div className="field">
          <label className="label" htmlFor="hub">
            Hub
          </label>
          <div className="control">
            <input
              disabled={running}
              type="text"
              className={`input ${hubClass}`}
              id="hub"
              value={hub}
              onChange={(e) => {
                setHub(e.target.value);
                setHubDirty(true);
              }}
              aria-invalid={!hubValid}
            />
            {!hubValid && hubDirty && (
              <p className="help is-danger">Hub is required</p>
            )}
          </div>
        </div>
        <div className="field">
          <div className="control">
            <label className="checkbox">
              <input
                disabled={running}
                type="checkbox"
                id="for-disposal"
                checked={forDisposal}
                onChange={(e) => setForDisposal(e.target.checked)}
              />{" "}
              For Disposal
            </label>
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

export const Route = createFileRoute("/vehicles/edit/$id")({
  component: EditVehicle,
  loader: ({ params, context }) => {
    return context.queryClient.ensureQueryData(vehicleSettings(params.id));
  },
});
