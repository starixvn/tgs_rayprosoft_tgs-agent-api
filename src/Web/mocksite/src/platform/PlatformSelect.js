import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
    Table,
    Set,
    Button,
    Stack,
    Box,
    PageWithHeader,
    TopNav,
    DropdownMenu,
    Group,
    Card,
    Select
} from "bumbag"
import { platformOptions } from "./index"

export const PlatformSelect = ({ name = "platformtype", ...props }) => {
    let options = platformOptions()
    return <Select name={name} options={options} {...props} />
}
