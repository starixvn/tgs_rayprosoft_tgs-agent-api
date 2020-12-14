import React, { useState } from "react"
import { css } from "@emotion/react"
import { useAuth } from "./context"
import { Formik, Field } from "formik"
import {
  Flex,
  Box,
  Card,
  Set,
  Text,
  InputField,
  Button,
  Dialog,
  Modal
} from "bumbag"
import AlertDialog from "../dialog/Alert"

export const Path = "/login"

export const Form = () => {
  const login = useAuth()

  const dialog = Modal.useState()
  let [error, setError] = useState()

  return (
    <Box
      alignX="center"
      alignY="center"
      css={css`
        height: 100vh;
      `}
    >
      <Card
        title="Login"
        css={css`
          width: 370px;
          @media screen and (max-width: 576px) {
            width: 95%;
          }
        `}
      >
        <Formik
          initialValues={{ username: "", password: "" }}
          onSubmit={(data) => {
            setError(null)

            login(data).catch((error) => {
              setError(error.message ?? error.title)
              dialog.show()
            })
          }}
        >
          {({ errors, touched, ...formProps }) => (
            <form
              onReset={formProps.handleReset}
              onSubmit={formProps.handleSubmit}
            >
              <Set orientation="vertical" isFilled>
                <Field
                  component={InputField.Formik}
                  name="username"
                  placeholder="Username"
                  autoComplete="username"
                />
                <Text>
                  {errors.username && touched.username && errors.username}
                </Text>
                <Field
                  component={InputField.Formik}
                  name="password"
                  type="password"
                  placeholder="Password"
                  autoComplete="current-password"
                />
                <Text>
                  {errors.password && touched.password && errors.password}
                </Text>
                <Button type="submit" palette="primary">
                  Login
                </Button>
              </Set>
            </form>
          )}
        </Formik>
      </Card>
      <ErrorDialog error={error} dialog={dialog} />
    </Box>
  )
}

//TODO: use alert dialog
const ErrorDialog = ({ dialog, error, ...props }) => {
  return (
    <Dialog.Modal
      type="danger"
      variant="alert"
      title={error}
      placement="top"
      footer={
        <Flex
          alignX="right"
          css={css`
            width: 100%;
          `}
        >
          <Modal.Disclosure use={Button} {...dialog}>
            Close
          </Modal.Disclosure>
        </Flex>
      }
      {...dialog}
    ></Dialog.Modal>
  )
}
