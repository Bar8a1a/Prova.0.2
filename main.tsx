import ReactDOM from 'react-dom/client'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import './index.css'
import Cadastrar from './pages/consumo/cadastrar'
import Listar from './pages/consumo/listar'
import ListarPorStatus from './pages/consumo/listarporstatus'
import Alterar from './pages/consumo/alterar'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <BrowserRouter>cade a
      <Routes>
        <Route path="/pages/consumo/cadastrar" element={<Cadastrar />} />
        <Route path="/pages/consumo/listar" element={<Listar />} />
        <Route path="/pages/consumo/listarporstatus" element={<ListarPorStatus />} />
        <Route path="/pages/consumo/alterar" element={<Alterar />} />
      </Routes>
    </BrowserRouter>
  </React.StrictMode>,
)

import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import './index.css'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
)
