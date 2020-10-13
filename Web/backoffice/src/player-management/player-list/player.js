import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/core"
import Async from "react-async"
import { useLocation, useRouteMatch } from "react-router-dom"
import useSWR from "swr"
import { usePlayer, useEditPlayerInfo } from "./client"
import { Formik, Field, Form, useFormikContext } from "formik"
import {
  useTabState,
  Tab as _Tab,
  TabList,
  TabPanel as _TabPanel
} from "reakit/Tab"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Label,
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  Select as _Select,
  Tabs as _Tabs,
  Dialog,
  Modal,
  Table,
  useClassName,
  ActionButtons
} from "bumbag"
import { FieldValues } from "../../layout/Grid"
import { Trans, FormattedDateTime, FormattedNumber } from "../../i18n"
import { Field as InputField, Password as PasswordField } from "../../input/Field"
import { Field as StatusField } from "../status"
import { Field as VipLevelField } from "../../vip-level/field"
import { useVipLevels } from "../../vip-level/client"
import { Field as _Field, FieldMinHeight } from "../../select/Field"
import AlertDialog from "../../dialog/Alert"

export const Key = "player"

export const Tab = ({ ...props }) => {
  let tabProps = _Tabs.Tab.useProps()
  return (
    <_Tab {...tabProps} {...props} id={Key}>
      <Trans zh="玩家信息" en="Player" />
    </_Tab>
  )
}

export const EditPanel = ({ player = {}, playerCode = "", onSubmitted, ...props }) => {
  let [submitData, setSubmit] = useState()
  let { result, error, mutate } = useEditPlayerInfo({ playerCode, data: submitData })

  const submit = (data = {}) => {
    let hasAgent = data.hasAgent === Boolean(true).toString() ? true : false
    setSubmit({
      ...data,
      hasAgent
    })
  }

  const clearResult = () => {
    setSubmit(null)
    mutate()
  }

  let {
    items: vipLevels,
  } = useVipLevels()

  let {
    username = "",
    realName = "",
    vipLevel = "",
    parentUsername = "",
    registeredOn,
    balance,
    status,
    hasAgent
  } = player
  hasAgent = hasAgent.toString()
  //TODO: vipId from API?
  let vip = vipLevels.find(
    item => item.name === vipLevel
  ) ?? ""

  let vipId = vip.id

  useEffect(() => {
    console.log("TODO: apply onSubmitted ?")
  }, [result])
  return (
    <_TabPanel {...props} id={Key}>
      {error && <AlertDialog {...error} onClickClose={clearResult}/>}
      <Formik 
        initialValues={{
          realName,
          vipId,
          parentUsername: parentUsername ?? "",
          status,
          hasAgent
        }}
        onSubmit={submit}
        >
          {formikProps => (
            <form onReset={formikProps.handleReset} onSubmit={formikProps.handleSubmit} >
              <FieldValues margin="major-1">
                <UserNameLabel />
                <Text>{username}</Text>

                <RealNameLabel />
                <Box>
                  <InputField name="realName" required />
                </Box>
                
                <VIPLevelLabel />
                <Box>
                  <VipLevelField name="vipId" required/>
                </Box>
                
                <ParentUsernameLabel />
                <Box>
                  <InputField name="parentUsername"  />
                </Box>

                <RegistrationLabel />
                <Box>
                  <FormattedDateTime value={registeredOn} />
                </Box>

                <BalanceLabel />
                <Box>
                  <FormattedNumber value={balance} />
                </Box>
                
                <StatusLabel />
                <Box>
                  <StatusField name="status" required/>
                </Box>
                
                <AgentLabel />
                <Box>
                    <_Field name="hasAgent" required>
                      {(props) => <_Select options={[
                        { label: "开启", value: true }, 
                        { label: "关闭", value: false }
                        ]} {...props} />}                  
                    </_Field>
                </Box>
              </FieldValues>      
              <Box alignX="right">
                <ActionButtons 
                  onClickSubmit={() => {
                    formikProps.submitForm()
                    //submit(formikProps.values)
                  }}
                  onClickCancel={() => {
                    console.log('cancelled')
                  }}
                  /> 
              </Box>  
            </form>
          )}
      </Formik>
    </_TabPanel>
  )
}

export const Panel = ({ player = {}, onSubmitted, ...props }) => {
  let {
    username = "",
    realName = "",
    vipLevel = "",
    parentUsername = "",
    registeredOn,
    balance,
    status,
    hasAgent
  } = player
  
  return (
    <_TabPanel {...props} id={Key}>
      <FieldValues margin="major-1">
        <UserNameLabel />
          <Text>{username}</Text>

          <RealNameLabel />
          <Box>
            <Text>{realName}</Text>
          </Box>
          
          <VIPLevelLabel />
          <Box>
            <Text>{vipLevel}</Text>
          </Box>
          
          <ParentUsernameLabel />
          <Box>
            <Text>{parentUsername}</Text>
          </Box>

          <RegistrationLabel />
          <Box>
            <FormattedDateTime value={registeredOn} />
          </Box>

          <BalanceLabel />
          <Box>
            <FormattedNumber value={balance} />
          </Box>
          
          <StatusLabel />
          <Box>
            <StatusLabel value={status} />
          </Box>
          
          <AgentLabel />
          <Box>
            <Text>
              {hasAgent && <Trans zh="开启" en="Turn on" />}
              {!hasAgent && <Trans zh="关闭" en="Turn off" />}
            </Text>
          </Box>
        </FieldValues>
    </_TabPanel>
  )
}

const UserNameLabel = () => <Trans zh="用户名" en="User name" />

const RealNameLabel = () => <Trans zh="真实姓名" en="Real name" />

const VIPLevelLabel = () => <Trans zh="VIP等级" en="VIP Level" />

const ParentUsernameLabel = () => <Trans zh="上级用户名" en="上级用户名" />

const RegistrationLabel = () => <Trans zh="注册时间" en="Registration" />

const BalanceLabel = () => <Trans zh="余额" en="Balance" />

const StatusLabel = () => <Trans zh="状态" en="Status" />

const AgentLabel = () => <Trans zh="代理功能" en="Agent" />