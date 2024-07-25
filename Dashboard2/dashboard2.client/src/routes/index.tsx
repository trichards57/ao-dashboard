import { createFileRoute, redirect } from "@tanstack/react-router";
import { useTitle } from "../components/useTitle";

export function Index() {
  useTitle();

  return (
    <section className="hero">
      <div className="hero-body">
        <p className="title">Welcome</p>
        <p className="subtitle">
          Welcome to the AO Ambulance Dashboard. You will need to log in to use
          this application.
        </p>
      </div>
      <div className="hero-foot is-flex is-justify-content-center">
        <a className="button is-primary" href="/Identity/Account/Login">
          Log In
        </a>
      </div>
    </section>
  );
}

export const Route = createFileRoute("/")({
  beforeLoad: ({ context }) => {
    if (context.loggedIn) {
      throw redirect({
        to: "/home",
      });
    }
  },
  component: Index,
});
