import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { SideNav } from "bumbag"
import {
  Switch,
  Route,
  Redirect,
  Link,
  useParams,
  useRouteMatch,
  useLocation
} from "react-router-dom"
import { useBrands } from "./client/brand"

const DefaultBrandCode = "TGS"

const Sidebar = () => {
  let brands = useBrands()
  let match = useRouteMatch({
    path: "/:brandCode",
    sensitive: false
  })
  if (!match) {
    return <Redirect to={`/${DefaultBrandCode}`} push={false} />
  }
  let { params } = match
  return (
    <SideNav selectedId={params.brandCode}>
      <SideNav.Level>
        {brands.map((brand, index) => (
          <BrandNav key={index} brand={brand} />
        ))}
      </SideNav.Level>
    </SideNav>
  )
}

const BrandNav = ({ brand }) =>
  brand?.brandCode ? (
    <SideNav.Item
      use={Link}
      to={`/${brand?.brandCode}`}
      navId={`${brand?.brandCode}`}
    >
      {brand?.brandCode}
    </SideNav.Item>
  ) : undefined

export default Sidebar
