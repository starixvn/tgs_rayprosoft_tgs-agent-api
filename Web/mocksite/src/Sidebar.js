import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { SideNav } from "bumbag"

const Sidebar = () => {
  return (
    <SideNav>
      <SideNav.Level title="Reseller A">
        <SideNav.Level title="Licensee A">
          <SideNav.Level>
            <SideNav.Item>Brand A</SideNav.Item>
          </SideNav.Level>
        </SideNav.Level>
      </SideNav.Level>
      <SideNav.Level title="Reseller B2B">
        <SideNav.Level title="Licensee B2B">
          <SideNav.Level>
            <SideNav.Item>Brand B2B</SideNav.Item>
          </SideNav.Level>
        </SideNav.Level>
      </SideNav.Level>
    </SideNav>
  )
}

export default Sidebar
