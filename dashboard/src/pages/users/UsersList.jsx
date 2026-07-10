import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { listUsers } from "../../api/users";
import PageHeader from "../../components/PageHeader";
import { Button } from "../../components/Form";
import { Loading, ErrorBlock, Empty } from "../../components/Status";

export default function UsersList() {
  const [state, setState] = useState({ loading: true, error: null, users: [] });

  const load = () => {
    setState((s) => ({ ...s, loading: true, error: null }));
    listUsers()
      .then((users) => setState({ loading: false, error: null, users: users ?? [] }))
      .catch((err) => setState((s) => ({ ...s, loading: false, error: err.message })));
  };

  useEffect(load, []);

  return (
    <div>
      <PageHeader
        eyebrow="People"
        title="Users"
        action={
          <Link to="/users/new">
            <Button>New user</Button>
          </Link>
        }
      />

      {state.loading && <Loading label="Loading users…" />}
      {state.error && <ErrorBlock message={state.error} onRetry={load} />}

      {!state.loading && !state.error && state.users.length === 0 && (
        <Empty
          title="No users yet"
          hint="Add the first person to start tracking their spending."
          action={
            <Link to="/users/new">
              <Button>New user</Button>
            </Link>
          }
        />
      )}

      {!state.loading && !state.error && state.users.length > 0 && (
        <div className="overflow-hidden rounded-sm border border-line bg-white">
          <table className="w-full text-left text-sm">
            <thead>
              <tr className="rule text-xs uppercase tracking-wide text-muted">
                <th className="px-4 py-3 font-medium">Name</th>
                <th className="px-4 py-3 font-medium">Email</th>
                <th className="px-4 py-3 font-medium">Joined</th>
              </tr>
            </thead>
            <tbody>
              {state.users.map((user) => (
                <tr key={user.id} className="rule last:border-b-0 hover:bg-paper-dim/50">
                  <td className="px-4 py-3">
                    <Link
                      to={`/users/${user.id}`}
                      className="font-medium text-ink hover:underline"
                    >
                      {user.name}
                    </Link>
                  </td>
                  <td className="px-4 py-3 text-ink-soft">{user.email}</td>
                  <td className="px-4 py-3 font-mono text-xs text-muted">
                    {user.createdAt
                      ? new Date(user.createdAt).toLocaleDateString()
                      : "—"}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
