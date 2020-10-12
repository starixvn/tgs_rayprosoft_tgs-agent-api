import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { get } from "../client"
import { Select as _Select } from "bumbag"
import { Field as _Field } from "../select/Field"
import AlertDialog from "../dialog/Alert"
import { useVipLevels } from "./client"

export const Field = ({ required, validate, ...props }) => {
  let { name } = props
  return (
    <_Field name={name} required={required}>
      {(props) => <Select required={required} {...props} />}
    </_Field>
  )
}

export const Select = ({ required, ...props }) => {
  let { items, isLoading, error } = useVipLevels()

  if (isLoading) {
    return <_Select isLoading options={[]} {...props} />
  }

  if (error) {
    return <AlertDialog {...error}/>
  }

  let optioniItems = items.map(({ id, name }) => ({
    label: name,
    value: id
  }))

  let options = required 
  ? optioniItems
  : [
    { label: "", value: "" },
    ...optioniItems
  ]

  return (
    <_Select
      options={options}
      {...props}
    />
  )
}
