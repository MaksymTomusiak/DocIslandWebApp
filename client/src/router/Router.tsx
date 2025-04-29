import { BrowserRouter, Routes, Route } from "react-router-dom";
import NotFoundPage from "../components/common/NotFoundPage";
import Layout from "../components/layout/Layout";

const Router = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="*" element={<NotFoundPage />} />
        <Route path="/" element={<Layout />}></Route>
      </Routes>
    </BrowserRouter>
  );
};

export default Router;
