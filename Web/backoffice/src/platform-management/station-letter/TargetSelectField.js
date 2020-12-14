import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import {
  Select as _Select,
  Box,
  Set,
  Button,
  Text,
  Switch,
  Input,
  Stack
} from "bumbag"
import _Field from "../../select/Field"
import { useTrans } from "../../i18n"

export const AllPlayers = "AllPlayers"
const AllPlayersTrans = {
  zh: "全部玩家",
  en: "All players"
}

export const DesignatedPlayers = "DesignatedPlayers"
const DesignatedPlayersTrans = {
  zh: "指定玩家",
  en: "Designated Players"
}

export const Option = {
  AllPlayers,
  DesignatedPlayers
}

const Select = ({ required, ...props }) => {
  let options = [
    {
      label: useTrans(AllPlayersTrans),
      value: AllPlayers
    },
    {
      label: useTrans(DesignatedPlayersTrans),
      value: DesignatedPlayers
    }
  ]
  if (!required) {
    options = [{ label: "", value: "" }, ...options]
  }

  return <_Select options={options} {...props} />
}

export const Field = ({ name, required, disabled, ...props }) => {
  return (
    <_Field name={name} required={required}>
      {(props) => <Select required={required} disabled={disabled} {...props} />}
    </_Field>
  )
}

export default Field
