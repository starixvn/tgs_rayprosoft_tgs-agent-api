import React, { useState, useEffect, useMemo } from "react"
import { css, Stack, Heading, Box, Button } from "bumbag"
import { useFormikContext } from "formik"
import { useChangeProviderStatus } from "./client"
import Alert from "../../dialog/Alert"

export const Label = ({ value = "" }) => {
  console.warn("TODO: Label")
  switch (value) {
    default:
      return value
  }
}

const statusPalette = (status) => {
  console.warn("TODO: statusPalette")
  switch (status) {
    default:
      return "default"
  }
}

const toggleStatus = (status) => {
  console.warn("TODO: toggleStatus")
  switch (status) {
    default:
      return status
  }
}

export const ToggleButton = ({ record, disabled, ...props }) => {
  let [newStatus, setStatus] = useState()
  let { result, isLoading, error, deleteCache } = useChangeProviderStatus(
    record?.id,
    newStatus
  )

  const reset = () => {
    deleteCache()
    setSubmit(undefined)
  }

  useEffect(() => {
    if (result && !error) {
      reset()
      onSubmitted && onSubmitted()
    }
  }, [result, error])

  return (
    <>
      <Button
        onClick={() => {
          console.warn("TODO: ToggleButton onClick")
          reset()
          let newStatus = toggleStatus("TODO")
          setStatus(newStatus)
        }}
        isLoading={isLoading}
        disabled={isLoading || disabled}
        palette={statusPalette("TODO: statusPalette")}
        whiteSpace="nowrap"
      >
        <Label value={"TODO: ToggleButton Label"} />
      </Button>
      {error && <Alert {...error} onClickClose={reset} />}
    </>
  )
}

export const ToggleButtonField = ({ name }) => {
  console.warn("TODO: ToggleButtonField default name")
  let { values = {}, setFieldValue } = useFormikContext()
  let status = Reflect.get(values, name)

  return (
    <Button
      type="button"
      palette={statusPalette(status)}
      onClick={() => {
        let newStatus = toggleStatus(status)
        setFieldValue(name, newStatus)
      }}
    >
      <Label value={status} />
    </Button>
  )
}
