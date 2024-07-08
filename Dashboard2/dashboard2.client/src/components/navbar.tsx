import { Link, useSearch } from "@tanstack/react-router";
import { useState } from "react";

interface NavbarProps {
  loggedIn?: boolean;
  canViewVor?: boolean;
  canEditVehicles?: boolean;
  canViewUsers?: boolean;
  canEditRoles?: boolean;
  name?: string;
}

export default function Navbar({
  loggedIn = false,
  canViewVor = false,
  canEditRoles = false,
  canEditVehicles = false,
  canViewUsers = false,
  name = undefined,
}: NavbarProps) {
  const [showMenu, setShowMenu] = useState(false);
  const search = useSearch({ strict: false });

  const burgerClass = showMenu ? "navbar-burger is-active" : "navbar-burger";
  const menuClass = showMenu ? "navbar-menu is-active" : "navbar-menu";

  return (
    <nav className="navbar" role="navigation" aria-label="main navigation">
      <div className="navbar-brand">
        <Link className="navbar-item" to={loggedIn ? "/home" : "/"}>
          AO Dashboard
        </Link>
        <a
          onClick={() => setShowMenu((s) => !s)}
          className={burgerClass}
          role="button"
          aria-label="menu"
          aria-expanded="false"
        >
          <span aria-hidden="true"></span>
          <span aria-hidden="true"></span>
          <span aria-hidden="true"></span>
          <span aria-hidden="true"></span>
        </a>
      </div>
      <div className={menuClass}>
        <div className="navbar-start">
          {canViewVor &&
            (loggedIn ? (
              <Link to="/home" search={search} className="navbar-item">
                Home
              </Link>
            ) : (
              <Link to="/" className="navbar-item">
                Home
              </Link>
            ))}
          {canViewVor && (
            <Link className="navbar-item" to="/vehicles/status" search={search}>
              Vehicle Status
            </Link>
          )}
          {canEditVehicles && (
            <Link className="navbar-item" to="/vehicles/config" search={search}>
              Vehicle Setup
            </Link>
          )}
          {canViewUsers && (
            <Link className="navbar-item" to="/users">
              User Settings
            </Link>
          )}
          {canEditRoles && (
            <Link className="navbar-item" to="/roles">
              Role Settings
            </Link>
          )}
        </div>

        <div className="navbar-end">
          {loggedIn && (
            <div className="navbar-item has-dropdown is-hoverable">
              <a className="navbar-link">{name}</a>
              <div className="navbar-dropdown">
                <a className="navbar-item" href="/Account/Manage">
                  Manage Profile
                </a>
                <hr className="navbar-divider" />
                <div className="navbar-item">
                  <form method="post" action="Account/Logout">
                    <input type="hidden" name="returnUrl" value="" />
                    <button type="submit">Log Out</button>
                  </form>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </nav>
  );
}
