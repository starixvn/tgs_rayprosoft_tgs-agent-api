import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync } from "react-async"
import { Formik, Field } from "formik"
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

//TODO:
// 1. get all department options
// 2. get all group options
export default ({}) => {
  //const dialog = Modal.useState()

  const onSubmit = (data) => {
    console.log("submit", data)
  }

  return (
    <Modal.State>
      <Modal.Disclosure use={Button}>{add()}</Modal.Disclosure>
      <Dialog.Modal title={title()} showActionButtons>
        TODO: form
      </Dialog.Modal>
    </Modal.State>
  )
}

//TODO:
// 1. submit
// 2. error handling
const Form = ({ dialog, onSubmit }) => {
  return (
    <Box>
      <Formik
        initialValues={{
          username: "",
          departmentName: "",
          role: "",
          password: ""
        }}
        onSubmit={onSubmit}
      >
        {(formProps) => {
          ;<form
            onReset={formProps.handleReset}
            onSubmit={formProps.handleSubmit}
          >
            <Set>
              <Field
                component={InputField.Formik}
                name="username"
                label={username()}
                autocomplete="off"
              />
              <Field component={SelectField.Formik} name="departmentName" />
              <Field component={SelectField.Formik} name="role" />
              <Field
                component={InputField.Formik}
                name="password"
                type="password"
                label={password()}
              />
              <Field
                component={InputField.Formik}
                name="confirmPassword"
                type="password"
                label={confirmPassword()}
              />
            </Set>
          </form>
        }}
      </Formik>
    </Box>
  )
}

//TODO: i18n
const title = () => "新增管理员"

const add = () => "新增"

const username = () => "用户名"

const password = () => "密码"

const confirmPassword = () => "确认密码"
