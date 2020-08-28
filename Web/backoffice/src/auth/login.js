import React from "react"
import { useLogin } from "./context"
import { Formik, Field } from "formik"
import { Flex, Card, Set, Text, InputField, Button } from "bumbag"

export const Form = () => {
  const login = useLogin()

  return (
    <Card title="Login">
      <Formik
        initialValues={{ username: "", password: "" }}
        onSubmit={(data) => login(data)}
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
              />
              <Text>
                {errors.username && touched.username && errors.username}
              </Text>
              <Field
                component={InputField.Formik}
                name="password"
                type="password"
                placeholder="Password"
              />
              <Text>
                {errors.password && touched.password && errors.password}
              </Text>
              <Button type="submit" palette="primary">
                TODO: login and error dialog
              </Button>
            </Set>
          </form>
        )}
      </Formik>
    </Card>
  )
}
