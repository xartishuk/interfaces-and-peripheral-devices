hdd_info =  %x'sudo hdparm -I /dev/sda'

model = hdd_info.scan(/Model Number:.+/)
firmare = hdd_info.scan(/Firmware Revision:.+/)
serial_number = hdd_info.scan(/Serial Number:.+/)
transport = hdd_info.scan(/Transport:.+/)
pio = hdd_info.scan(/PIO:.+/)
dma = hdd_info.scan(/DMA:.+/)
disk_size = hdd_info.scan(/device size with M \= 1024\*1024:.+/).first.split(/:/).last
puts model + firmare + serial_number + transport + pio + dma
puts  'Hard Disk size:' + disk_size

df_info = %x'df -h --total'
free_memory = df_info.scan(/total.+/).first.split(/G/)[2]
puts 'Free memory:' + free_memory + ' Gb'
