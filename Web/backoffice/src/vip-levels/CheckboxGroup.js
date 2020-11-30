import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/core"
import { Formik, Form, Field as _Field } from "formik"
import { CheckboxGroupField } from "bumbag"
import AlertDialog from "../dialog/Alert"
import { useVipLevelList } from "./client"

export const Field = ({ orientation = "horizontal", ...props }) => {
  let { items = [], isLoading, error } = useVipLevelList()

  let options = items.map(({ id, name, ...item }) => ({
    label: name,
    value: id
  }))

  return (
    <>
      <_Field
        component={CheckboxGroupField.Formik}
        orientation={orientation}
        options={options}
        {...props}
      />
      {error && <AlertDialog {...error} />}
    </>
  )
}
