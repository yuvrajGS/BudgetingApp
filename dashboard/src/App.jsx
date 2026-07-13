import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";
import Dashboard from "./pages/Dashboard";

import UsersList from "./pages/users/UsersList";
import UserDetail from "./pages/users/UserDetail";
import UserCreate from "./pages/users/UserCreate";

import CategoriesList from "./pages/categories/CategoriesList";
import CategoryDetail from "./pages/categories/CategoryDetail";
import CategoryCreate from "./pages/categories/CategoryCreate";

import TransactionsList from "./pages/transactions/TransactionsList";
import TransactionDetail from "./pages/transactions/TransactionDetail";
import TransactionCreate from "./pages/transactions/TransactionCreate";
import TransactionBatchCreate from "./pages/transactions/TransactionBatchCreate";
import TransactionImport from "./pages/transactions/TransactionImport";
import TransactionUpdate from "./pages/transactions/TransactionUpdate";

export default function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<Dashboard />} />

          <Route path="/users" element={<UsersList />} />
          <Route path="/users/new" element={<UserCreate />} />
          <Route path="/users/:id" element={<UserDetail />} />

          <Route path="/categories" element={<CategoriesList />} />
          <Route path="/categories/new" element={<CategoryCreate />} />
          <Route path="/categories/:name" element={<CategoryDetail />} />

          <Route path="/transactions" element={<TransactionsList />} />
          <Route path="/transactions/new" element={<TransactionCreate />} />
          <Route path="/transactions/batch" element={<TransactionBatchCreate />} />
          <Route path="/transactions/import" element={<TransactionImport />} />
          <Route path="/transactions/:id" element={<TransactionDetail />} />
          <Route path="/transactions/update/:id" element={<TransactionUpdate />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  );
}
