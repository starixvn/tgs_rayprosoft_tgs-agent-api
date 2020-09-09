import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync } from "react-async"
import client from "../../client"
import { Formik, Field, useFormik } from "formik"
import {
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  InputField,
  SelectField,
  Dialog,
  Modal
} from "bumbag"

const DefaultState = {
  name: ""
}

const useFormState = (initialValues = DefaultState) => {
  let state = useFormik({
    initialValues,
    onSubmit: (data) => {
      console.log("submit", data)
    }
  })

  return state
}

const Form = ({ state }) => {
  return (
    <form onReset={state.handleReset} onSubmit={state.handleSubmit}>
      <Set>
        <Field
          component={InputField.Formik}
          name="name"
          label={name()}
          autocomplete="off"
        />
      </Set>
    </form>
  )
}

export const AddButton = ({}) => {
  let [promise, setPromise] = useState(Promise.resolve())
  let form = useFormState()
  let async = useAsync({ promise })

  return (
    <Modal.State>
      <Modal.Disclosure use={Button}>{add()}</Modal.Disclosure>
      <Dialog.Modal
        title={title()}
        showActionButtons
        actionButtonsProps={{
          onClickSubmit: (data) => {
            console.log("actionButtons onClickSubmit", data)
          }
        }}
      >
        <Form state={form} />
      </Dialog.Modal>
    </Modal.State>
  )
}

export const EditButton = ({}) => {}

//TODO: i18n
const add = () => "新增"

const title = () => "新增权限组"

const name = () => "权限组名称"
