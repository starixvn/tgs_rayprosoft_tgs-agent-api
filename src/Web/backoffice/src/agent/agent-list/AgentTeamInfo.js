import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import { Spinner, Stack, Flex, Box, Card, Set } from "bumbag"
import { FieldValues } from "../../layout/Grid"
import { Trans, FormattedDateTime, FormattedNumber, useTrans } from "../../i18n"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"
import { FormProvider, useForm, useFormContext } from "react-hook-form"
import Input from "../../input/Input"
import { pastWeek } from "../../date/range"
import { Table, useTable, toApiParams, pageCount } from "../../table"
import dateTimeColumn from "../../table/column/dateTime"
import textColumn from "../../table/column/textColumn"
import DateTimeRangeInput from "../../date/DateTimeRangeInput"
import SearchButton from "../../button/SearchButton"
import isSameDate from "date-fns/isEqual"
import { Ascending, Descending } from "../../table/order"
import { coloredNumberDecimalColumn } from "../../table/column/numberColumn"
import { useAgentTeamInfo } from "../../report/agent-contribution-report/client"

const Key = "AgentTeamInfo"

export const AgentTeamInfoTab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="团队资讯" en="Team Info" />
        </_Tab>
    )
}

export const AgentTeamInfoPanel = ({ agentId, record, ...props }) => {
    let { selectedId } = props
    let isActive = selectedId === Key

    let { state, setState, setPageIndex, setSortOrder } = useTable({})

    let { pageSize } = state
    let { isLoading, data, error, mutate, deleteCache } = useAgentTeamInfo(
        isActive ? agentId : undefined,
        toApiParams({ ...state, sortby: "username", orderby: Ascending })
    )

    let columns = useColumns({ items: data?.items })

    return (
        <TabPanel tabId={Key} {...props}>
            <Table
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                initialState={state}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate(undefined, false)
                    }}
                />
            )}
        </TabPanel>
    )
}

const useColumns = ({ items = [] }) =>
    useMemo(
        () => [
            textColumn({
                accessor: "username",
                Header: <Trans zh="用户名" en="Username" />,

                disableSortBy: true
            }),
            dateTimeColumn({
                accessor: "registeredOn",
                Header: <Trans zh="注册时间" en="Registration Time" />,
                disableSortBy: true
            }),
            coloredNumberDecimalColumn({
                accessor: "contribution",
                Header: <Trans zh="贡献值" en="Contribution" />,
                disableSortBy: true
            })
        ],
        [JSON.stringify(items)]
    )
