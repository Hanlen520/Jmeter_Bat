@echo off

::---------�����ļ�-------------
::jmx�ļ���
set JMX_NAME=1V1_WebSocket
::��ʼ��ʶֵ
set index=1
::------------------------------

::���ɵ�ǰ����
set dateTmp=%date:~0,4%%date:~5,2%%date:~8,2%
if "%time:~0,2%" lss "10" (set hour=0%time:~1,1%) else (set hour=%time:~0,2%)
set timeTmp=%hour%%time:~3,2%%time:~6,2%%time:~9,2%
set d=%dateTmp%%timeTmp%
echo ��ǰʱ��: %d%

mkdir "%d%"

::ִ��Jmx�ű�
call jmeter -Jroom_index=%index%10000 -Jid_index=%index%1000 -Jteacher_id_index=%index%2000 -n -t %JMX_NAME%.jmx -l ./%d%/Jtl/%JMX_NAME%.csv -j %d%/jmeter.log -e -o ./%d%/Report
