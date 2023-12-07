import { ChangeEvent, FormEvent, useEffect, useState } from "react";
import useRequireLoggedIn from "../../hooks/useRequireLoggedIn";
import useVehicleConfig, {
  IVehicleConfig,
  Region,
  VehicleType,
} from "../../hooks/useVehicleConfig";
import useSetVehicleConfig from "../../hooks/useSetVehicleConfig";
import { Link } from "react-router-dom";

export default function Config() {
  useRequireLoggedIn();
  const [callSign, setCallSign] = useState("");
  const { data: vehicleData, isLoading: vehicleLoading } =
    useVehicleConfig(callSign);
  const {
    mutate: mutateVehicle,
    isSuccess: vehicleSuccess,
    reset: vehicleChangeReset,
  } = useSetVehicleConfig();
  const [displayedData, setDisplayedData] = useState(
    undefined as undefined | IVehicleConfig
  );
  const [registration, setRegistration] = useState("");
  const [region, setRegion] = useState("unknown" as Region);
  const [district, setDistrict] = useState("");
  const [vehicleType, setVehicleType] = useState("other" as VehicleType);
  const [saveRunning, setSaveRunning] = useState(false);

  useEffect(() => {
    if (!vehicleLoading && vehicleData) {
      setDisplayedData(vehicleData[0]);
    }
  }, [vehicleLoading, vehicleData]);

  useEffect(() => {
    setRegistration(displayedData?.reg ?? "");
    setRegion(displayedData?.region ?? "unknown");
    setDistrict(displayedData?.district ?? "");
    setVehicleType(displayedData?.type ?? "other");
  }, [displayedData]);

  useEffect(() => {
    setSaveRunning(false);
  }, [vehicleSuccess]);

  function changeCallSign(e: ChangeEvent<HTMLInputElement>) {
    setDisplayedData(undefined);
    setCallSign(e.currentTarget.value);
    vehicleChangeReset();
    setSaveRunning(false);
  }

  function submitSettings(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();

    if (!e.currentTarget.checkValidity()) {
      return;
    }

    setSaveRunning(true);

    mutateVehicle({
      callSign,
      district,
      reg: registration,
      region,
      type: vehicleType,
    });
  }

  return (
    <div className="card">
      <h1>Vehicle Configuration</h1>

      <form
        className="call-sign-form"
        noValidate
        onSubmit={(e) => {
          e.preventDefault();
          return false;
        }}
      >
        <label htmlFor="call-sign">Call Sign:</label>
        <input
          id="call-sign"
          type="text"
          placeholder="WR123"
          required
          pattern="^[a-zA-Z]{2}\d{3}$"
          value={callSign}
          onChange={changeCallSign}
          disabled={saveRunning}
        />
        <div className="validation-message">
          You need to give a valid vehicle callsign (e.g. WR123).
        </div>
      </form>
      {displayedData && (
        <form className="vehicle-form" onSubmit={submitSettings}>
          <label htmlFor="registration">Registration:</label>
          <input
            id="registration"
            type="text"
            placeholder="X632NBK"
            value={registration}
            onChange={(e) => {
              setRegistration(e.currentTarget.value);
              vehicleChangeReset();
            }}
            disabled={saveRunning}
            required
          />
          <div className="validation-message">
            You need to give a vehicle registration.
          </div>
          <label htmlFor="region">Region:</label>
          <select
            id="region"
            required
            value={region}
            onChange={(e) => {
              setRegion(e.currentTarget.value as Region);
              vehicleChangeReset();
            }}
            disabled={saveRunning}
          >
            <option value="unknown">Unknown</option>
            <option value="ne">North East</option>
            <option value="nw">North West</option>
            <option value="ee">East of England</option>
            <option value="wm">West Midlands</option>
            <option value="em">East Midlands</option>
            <option value="lo">London</option>
            <option value="sw">South West</option>
            <option value="se">South East</option>
          </select>
          <label htmlFor="district">District:</label>
          <input
            id="district"
            type="text"
            placeholder="District"
            value={district}
            onChange={(e) => {
              setDistrict(e.currentTarget.value);
              vehicleChangeReset();
            }}
            disabled={saveRunning}
            required
          />
          <div className="validation-message">
            You need to enter a District.
          </div>
          <label htmlFor="vehicle-type">Type:</label>
          <select
            id="vehicle-type"
            required
            value={vehicleType}
            disabled={saveRunning}
            onChange={(e) => {
              setVehicleType(e.currentTarget.value as VehicleType);
              vehicleChangeReset();
            }}
          >
            <option value="other">Other</option>
            <option value="frontline">Front Line</option>
            <option value="awd">All Wheel Drive</option>
            <option value="ora">Off Road Ambulance</option>
          </select>
          <button
            type="submit"
            disabled={vehicleLoading || saveRunning || vehicleSuccess}
          >
            Save
          </button>
        </form>
      )}
      <Link className="button" to="/home">
        Back
      </Link>
    </div>
  );
}
