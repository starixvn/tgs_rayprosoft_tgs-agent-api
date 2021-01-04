import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { CheckboxGroupField } from "bumbag"
import AlertDialog from "../dialog/Alert"
import { useVipLevelList } from "./client"
import { CheckboxGroup } from "../checkbox/CheckboxGroup"

export const VipLevelCheckboxGroup = ({
  orientation = "horizontal",
  ...props
}) => {
  let { items = [], isLoading, error } = useVipLevelList()

  let options = items.map(({ id, name, ...item }) => ({
    label: name,
    value: id
  }))

  return (
    <>
      <CheckboxGroup orientation={orientation} options={options} {...props} />
      {error && <AlertDialog {...error} />}
    </>
  )
}
