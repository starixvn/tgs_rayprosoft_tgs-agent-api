import React from "react"
import { Key as PermissionKey } from "./permission"

export const Key = "system"

export const DefaultUrl = `/${Key}`

const name = () => "System"

const DefaultKeys = []

export const Menu = ({
  title = name(),
  url = DefaultUrl,
  keys = DefaultKeys,
  ...props
}) => {}
