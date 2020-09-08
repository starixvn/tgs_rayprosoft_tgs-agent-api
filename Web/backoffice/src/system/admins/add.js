import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import Async, { useAsync } from "react-async"
import client from "../../client"
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
// 1. how to trigger form submit from action button
// 2. get all department options
// 3. get all group options
export default ({}) => {
  let async = useAsync({ deferFn: ([data]) => submit(data) })
  let { run: onClickSubmit, promise, isPending, error } = async

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
        <Form onSubmit={onClickSubmit} />
      </Dialog.Modal>
    </Modal.State>
  )
}

//TODO: return promise
const submit = (data) => {
  console.log("submit", data)
}

//TODO:
// 1. form data from parent?
// 2. submit
// 3. error handling
const Form = ({ onSubmit }) => {
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
        {(formProps) => (
          <form
            onReset={formProps.handleReset}
            onSubmit={formProps.handleSubmit}
          >
            <Set orientation="vertical">
              <Field
                component={InputField.Formik}
                name="username"
                isRequired
                label={username()}
                autocomplete="off"
              />
              <Field
                component={SelectField.Formik}
                name="departmentName"
                label={departmentName()}
                isRequired
                options={[]}
              />
              <Field
                component={SelectField.Formik}
                name="role"
                label={role()}
                isRequired
                options={[]}
                isRequired
              />
              <Field
                component={InputField.Formik}
                name="password"
                type="password"
                isRequired
                label={password()}
              />
              <Field
                component={InputField.Formik}
                name="confirmPassword"
                type="password"
                isRequired
                label={confirmPassword()}
              />
            </Set>
          </form>
        )}
      </Formik>
    </Box>
  )
}

//TODO: i18n
const title = () => "新增管理员"

const add = () => "新增"

const username = () => "用户名"

const departmentName = () => "权限组"

const role = () => "权限组"

const password = () => "密码"

const confirmPassword = () => "确认密码"
